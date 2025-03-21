using Microsoft.AspNetCore.Http;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Models.DTOs;
using NS.Shared.QueueManager.Models;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IUsersService : IBaseService<UserPayloadDto, UserDto>
    {
        Task<byte[]> DownloadAsync();
        Task ClearCacheAsync();
        Task<UploadFileResponse> UploadAsync(IFormFile file);
        Task<UploadFileStatusResponse> UploadFileStatusAsync(Guid uploadMessageId);
    }
}
