using Microsoft.AspNetCore.Http;
using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IUsersService : IBaseService<UserPayloadDto, UserDto>
    {
        Task<byte[]> DownloadAsync();
        Task UploadAsync(IFormFile file);
    }
}
