using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Services
{
    internal class AccountService : IAccountService
    {
        private readonly AppDbContext _appDbContext;
        private readonly JwtHelper _jwtHelper;

        public AccountService(AppDbContext appDbContext, JwtHelper jwtHelper)
        {
            _appDbContext = appDbContext;
            _jwtHelper = jwtHelper;
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

            return new LoginResponse()
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Token = _jwtHelper.GenerateToken(user.Id, user.Email, user.FullName),
            };
        }
    }
}
