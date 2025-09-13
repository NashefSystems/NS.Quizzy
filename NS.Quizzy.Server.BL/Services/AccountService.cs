using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.BL.Utils;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using NS.Shared.Logging.Extensions;
using static NS.Quizzy.Server.Common.Enums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class AccountService : IAccountService
    {
        private readonly INSLogger _logger;
        private readonly AppDbContext _appDbContext;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IOTPService _otpService;
        private readonly INSCacheProvider _cacheProvider;
        private readonly TimeSpan _cacheOTPTTL;
        private readonly TimeSpan _cacheLoginsTTL;
        private readonly IConfiguration _configuration;
        private readonly string _idNumberEmailDomain;

        public AccountService(INSLogger logger, AppDbContext appDbContext, JwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor, IOTPService otpService, INSCacheProvider cacheProvider, IConfiguration configuration)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
            _otpService = otpService;
            _cacheProvider = cacheProvider;
            _configuration = configuration;

            {
                var cacheKey = AppSettingKeys.CacheOTPTTLMin.GetDBStringValue();
                var valueInMin = double.TryParse(configuration.GetValue<string>(cacheKey), out double val) ? val : 60;
                _cacheOTPTTL = TimeSpan.FromMinutes(valueInMin);
            }
            {
                var cacheKey = AppSettingKeys.CacheLoginsTTLMin.GetDBStringValue();
                var valueInMin = double.TryParse(configuration.GetValue<string>(cacheKey), out double val) ? val : 20160;
                _cacheLoginsTTL = TimeSpan.FromMinutes(valueInMin);
            }
            {
                var cacheKey = AppSettingKeys.IdNumberEmailDomain.GetDBStringValue();
                _idNumberEmailDomain = configuration.GetValue<string>(cacheKey);
            }
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x =>
                x.IsDeleted == false &&
                x.Email.ToUpper() == loginRequest.Email.ToUpper()
            );

            if (user == null || user.Password != loginRequest.Password)
            {
                _logger.Error($"Login error", new { loginRequest, UserId = user?.Id, UserEmail = user?.Email });
                return null;
            }

            var res = new LoginResponse()
            {
                ContextId = _logger.GetContextId(),
                RequiresTwoFactor = true,
            };

            if (string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
            {
                var secretKey = _otpService.GenerateSecretKey();
                res.TwoFactorSecretKey = secretKey;
                res.TwoFactorUrl = _otpService.GetTwoFactorUrl(secretKey, user.Email);
                res.TwoFactorQrCode = _otpService.GenerateQRCode(res.TwoFactorUrl);
                _logger.Info($"Generated secret key");
            }

            var cacheInfo = new TwoFactorCacheInfo()
            {
                UserId = user.Id,
                TwoFactorSecretKey = res.TwoFactorSecretKey,
                Platform = loginRequest.Platform,
                DeviceId = loginRequest.DeviceId,
                AppVersion = loginRequest.AppVersion,
                NotificationToken = loginRequest.NotificationToken,
            };
            await _cacheProvider.SetOrUpdateAsync(GetTwoFactorCacheKey(res.ContextId), cacheInfo, _cacheOTPTTL);

            return res;
        }

        public string GetEmailForIdNumber(string IdNumber)
        {
            return $"{IdNumber}@{_idNumberEmailDomain}";
        }

        public async Task<UserDetailsDto> LoginWithIdNumberAsync(LoginWithIdNumberRequest loginRequest)
        {
            var email = GetEmailForIdNumber(loginRequest.IdNumber).ToUpper();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x =>
               x.IsDeleted == false &&
               (x.Role == DALEnums.Roles.Student || x.Role == DALEnums.Roles.Teacher) &&
               x.Email.ToUpper() == email
            );

            if (user == null)
            {
                _logger.Error($"LoginWithIdNumber | Login error", new { loginRequest });
                return null;
            }

            if (!string.IsNullOrWhiteSpace(loginRequest.NotificationToken))
            {
                user.NotificationToken = loginRequest.NotificationToken;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var loginHistoryItem = new LoginHistoryItem()
            {
                UserId = user.Id,
                NotificationToken = loginRequest.NotificationToken,
                DeviceId = loginRequest.DeviceId,
                AppVersion = loginRequest.AppVersion,
                ServerVersion = AppUtils.GetAppVersionAsString(),
                ClientIP = httpContext.GetClientIP(),
                IsMobile = !string.IsNullOrWhiteSpace(loginRequest.Platform) || httpContext.IsMobile(),
                Platform = StringUtils.FirstNotNullOrWhiteSpace(loginRequest.Platform, httpContext.GetPlatform()),
                UserAgent = httpContext.GetUserAgent(),
                Country = httpContext.GetCountryInfo()?.Name
            };

            var (tokenId, token) = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName, user.Role, loginHistoryItem.IsMobile);
            loginHistoryItem.Id = tokenId;
            loginHistoryItem.Token = token;

            await _appDbContext.LoginHistory.AddAsync(loginHistoryItem);
            await _appDbContext.SaveChangesAsync();

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(BLConsts.AUTH_TOKEN_KEY, token, new CookieOptions
            {
                HttpOnly = false, // If HttpOnly is true, JavaScript access to the cookie is prevented.
                Secure = true,   // Ensures the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents cross-site request forgery (CSRF)
                Expires = DateTime.UtcNow.AddMinutes(_jwtHelper.GetJwtExpiresInMinutes(loginHistoryItem.IsMobile)) // Set expiration time
            });

            _logger.Info($"LoginWithIdNumber | userId: '{user.Id}', fullName: '{user.FullName}', tokenId: '{tokenId}'");
            await AddLoginCacheAsync(user, tokenId, token);
            return new UserDetailsDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                TokenId = tokenId,
                Token = token,
                Role = user.Role,
                ClassId = user.ClassId,
                PushNotificationIsEnabled = !string.IsNullOrWhiteSpace(user.NotificationToken),
            };
        }

        private static string GetTwoFactorCacheKey(string requestId)
        {
            return $"TwoFactor:{requestId}";
        }

        public async Task<UserDetailsDto?> VerifyOTP(VerifyOTPRequest request)
        {
            var twoFactorCacheKey = GetTwoFactorCacheKey(request.Id);
            var cacheInfo = await _cacheProvider.GetAsync<TwoFactorCacheInfo?>(twoFactorCacheKey);
            if (cacheInfo?.UserId == null || cacheInfo.UserId == Guid.Empty)
            {
                return null;
            }

            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == cacheInfo.UserId && x.IsDeleted == false);
            if (user == null)
            {
                return null;
            }

            var cacheKey = AppSettingKeys.IgnoreOTPValidationUserIds.GetDBStringValue();
            var ignoreOTPValidationUserIdsJson = _configuration.GetValue<string>(cacheKey) ?? "[]";
            var ignoreOTPValidationUserIds = JsonConvert.DeserializeObject<List<Guid>>(ignoreOTPValidationUserIdsJson) ?? [];
            if (!ignoreOTPValidationUserIds.Contains(user.Id))
            {
                var twoFactorSecretKey = user.TwoFactorSecretKey;
                if (string.IsNullOrWhiteSpace(twoFactorSecretKey))
                {
                    twoFactorSecretKey = cacheInfo.TwoFactorSecretKey;
                }

                if (!_otpService.VerifyOTP(twoFactorSecretKey ?? "", request.Token))
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(user.TwoFactorSecretKey) && !string.IsNullOrWhiteSpace(cacheInfo.TwoFactorSecretKey))
                {
                    user.TwoFactorSecretKey = cacheInfo.TwoFactorSecretKey;
                    await _appDbContext.SaveChangesAsync();
                }
            }

            await _cacheProvider.DeleteAsync(twoFactorCacheKey);

            if (!string.IsNullOrWhiteSpace(cacheInfo.NotificationToken))
            {
                user.NotificationToken = cacheInfo.NotificationToken;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            var loginHistoryItem = new LoginHistoryItem()
            {
                UserId = user.Id,
                DeviceId = cacheInfo.DeviceId,
                AppVersion = cacheInfo.AppVersion,
                ServerVersion = AppUtils.GetAppVersionAsString(),
                NotificationToken = cacheInfo.NotificationToken,
                ClientIP = httpContext.GetClientIP(),
                IsMobile = !string.IsNullOrWhiteSpace(cacheInfo.Platform) || httpContext.IsMobile(),
                Platform = StringUtils.FirstNotNullOrWhiteSpace(cacheInfo.Platform, httpContext.GetPlatform()),
                UserAgent = httpContext.GetUserAgent(),
                Country = httpContext.GetCountryInfo()?.Name
            };

            var (tokenId, token) = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName, user.Role, loginHistoryItem.IsMobile);
            loginHistoryItem.Id = tokenId;
            loginHistoryItem.Token = token;

            await _appDbContext.LoginHistory.AddAsync(loginHistoryItem);
            await _appDbContext.SaveChangesAsync();

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(BLConsts.AUTH_TOKEN_KEY, token, new CookieOptions
            {
                HttpOnly = false, // If HttpOnly is true, JavaScript access to the cookie is prevented.
                Secure = true,   // Ensures the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents cross-site request forgery (CSRF)
                Expires = DateTime.UtcNow.AddMinutes(_jwtHelper.GetJwtExpiresInMinutes(loginHistoryItem.IsMobile)) // Set expiration time
            });

            _logger.Info($"VerifyOTP userId: '{user.Id}', fullName: '{user.FullName}', tokenId: '{tokenId}'");
            await AddLoginCacheAsync(user, tokenId, token);
            return new UserDetailsDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                TokenId = tokenId,
                Token = token,
                Role = user.Role,
                ClassId = user.ClassId,
                PushNotificationIsEnabled = !string.IsNullOrWhiteSpace(user.NotificationToken),
            };
        }

        private async Task AddLoginCacheAsync(User? user, Guid tokenId, string token)
        {
            var cacheData = new
            {
                user?.Id,
                user?.Email,
                user?.FullName,
                user?.IdNumber,
                user?.Role,
                tokenId,
                token
            };
            var cacheKey = $"Logins:{user?.Email}:{DateTime.Now:yyyy-MM-dd_HH-mm-ss}_{_logger.GetContextId()}";
            await _cacheProvider.SetOrUpdateAsync(cacheKey, cacheData, _cacheLoginsTTL);
        }

        public async Task<UserDetailsDto> GetDetailsAsync()
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var tokenId = _httpContextAccessor.HttpContext.GetTokenId();
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == userId);
            return new UserDetailsDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                IdNumber = user.IdNumber,
                TokenId = tokenId,
                Role = user.Role,
                ClassId = user.ClassId,
                PushNotificationIsEnabled = !string.IsNullOrWhiteSpace(user.NotificationToken),
            };
        }

        public Task LogoutAsync()
        {
            var userId = _httpContextAccessor.HttpContext.GetUserId();
            var tokenId = _httpContextAccessor.HttpContext.GetTokenId();
            _logger.Info($"Logout userId: '{userId}', tokenId: '{tokenId}'");
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(BLConsts.AUTH_TOKEN_KEY);
            return Task.CompletedTask;
        }
    }
}
