using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Models.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IAccountService
    {
        Task<UserDetailsDto> GetDetailsAsync();
        Task<LoginResponse?> LoginAsync(LoginRequest loginRequest);
        Task<UserDetailsDto> LoginWithIdNumberAsync(LoginWithIdNumberRequest loginRequest);
        Task<UserDetailsDto?> VerifyOTP(VerifyOTPRequest request);
        Task LogoutAsync();
        string GetEmailForIdNumber(string IdNumber);
    }
}
