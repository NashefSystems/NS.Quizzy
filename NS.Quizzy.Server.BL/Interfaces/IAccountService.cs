using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IAccountService
    {
        Task<UserDetailsDto> GetDetailsAsync();
        Task<UserDetailsDto?> LoginAsync(LoginRequest loginRequest);
        Task LogoutAsync();
    }
}
