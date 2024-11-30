using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.Models;
using System.IdentityModel.Tokens.Jwt;

namespace NS.Quizzy.Server.BL.Services
{
    internal class AccountService : IAccountService
    {
        private readonly AppDbContext _appDbContext;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountService(AppDbContext appDbContext, JwtHelper jwtHelper, IHttpContextAccessor httpContextAccessor)
        {
            _appDbContext = appDbContext;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = httpContextAccessor;
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
                return null;
            }
            var token = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(Consts.AUTH_TOKEN_KEY, token, new CookieOptions
            {
                HttpOnly = true, // Prevents JavaScript access to the cookie
                Secure = true,   // Ensures the cookie is sent only over HTTPS
                SameSite = SameSiteMode.Strict, // Prevents cross-site request forgery (CSRF)
                Expires = DateTime.UtcNow.AddMinutes(_jwtHelper.GetJwtExpiresInMinutes()) // Set expiration time
            });

            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
            };
        }

        public Task LogoutAsync()
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Delete(Consts.AUTH_TOKEN_KEY);
            return Task.CompletedTask;
        }
    }
}
