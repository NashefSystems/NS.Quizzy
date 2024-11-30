using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IAccountService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync();
    }
}
