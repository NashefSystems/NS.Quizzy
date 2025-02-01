using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.Models;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;

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

        public AccountService(INSLogger logger, AppDbContext appDbContext, JwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor, IOTPService otpService, INSCacheProvider cacheProvider)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
            _otpService = otpService;
            _cacheProvider = cacheProvider;
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
            };
            await _cacheProvider.SetOrUpdateAsync(GetTwoFactorCacheKey(res.ContextId), cacheInfo, TimeSpan.FromHours(1));

            return res;
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
            await _cacheProvider.DeleteAsync(twoFactorCacheKey);

            var (tokenId, token) = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(BLConsts.AUTH_TOKEN_KEY, token, new CookieOptions
            {
                HttpOnly = false, // If HttpOnly is true, JavaScript access to the cookie is prevented.
                Secure = true,   // Ensures the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents cross-site request forgery (CSRF)
                Expires = DateTime.UtcNow.AddMinutes(_jwtHelper.GetJwtExpiresInMinutes()) // Set expiration time
            });

            _logger.Info($"VerifyOTP userId: '{user.Id}', fullName: '{user.FullName}', tokenId: '{tokenId}'");
            return new UserDetailsDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                TokenId = tokenId,
                Token = token,
            };
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
                TokenId = tokenId
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
