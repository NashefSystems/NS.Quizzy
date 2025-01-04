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
                x.Email.ToUpper() == loginRequest.Email.ToUpper() &&
                x.Password == loginRequest.Password
            );

            if (user == null)
            {
                _logger.Error($"Login error", new { loginRequest });
                return null;
            }

            var res = new LoginResponse()
            {
                RequestId = _logger.GetGuid(),
                RequiresTwoFactor = true,
            };
            await _cacheProvider.SetOrUpdateAsync(GetOTPCacheKey(res.RequestId), user.Id, TimeSpan.FromHours(1));

            if (string.IsNullOrWhiteSpace(user.TwoFactorSecretKey))
            {
                user.TwoFactorSecretKey = _otpService.GenerateSecretKey();
                _appDbContext.SaveChanges();

                res.TwoFactorSecretKey = user.TwoFactorSecretKey;
                res.QrCode = _otpService.GenerateQRCode(user.TwoFactorSecretKey, user.Email);
                _logger.Info($"Generated secret key");
            }

            return res;
        }

        private static string GetOTPCacheKey(string requestId)
        {
            return $"OTP:Quizzy:{requestId}";
        }

        public async Task<UserDetailsDto?> VerifyOTP(VerifyOTPRequest request)
        {
            var userId = await _cacheProvider.GetAsync<Guid?>(GetOTPCacheKey(request.Id));
            if (userId == null || userId == Guid.Empty)
            {
                return null;
            }

            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.Id == userId && x.IsDeleted == false);
            if (user == null || string.IsNullOrWhiteSpace(user.TwoFactorSecretKey) || !_otpService.VerifyOTP(user.TwoFactorSecretKey, request.OTP))
            {
                return null;
            }

            var (tokenId, token) = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(BLConsts.AUTH_TOKEN_KEY, token, new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access to the cookie
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
            var user = await _appDbContext.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == userId);
            return new UserDetailsDto()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
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
