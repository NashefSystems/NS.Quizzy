using NS.Quizzy.Server.Models.DTOs;

namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IBaseService<T> where T : BaseEntityDto
    {
        Task<List<T>> GetAllAsync();
        Task<T> GetAsync(Guid id);
        Task<T> InsertAsync(T model);
        Task<T> UpdateAsync(Guid id, T model);
        Task DeleteAsync(Guid id);
    }
}
