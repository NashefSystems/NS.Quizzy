namespace NS.Quizzy.Server.BL.Interfaces
{
    public interface IBaseService<PayloadDto, Dto>
    {
        Task<List<Dto>> GetAllAsync();
        Task<Dto?> GetAsync(Guid id);
        Task<Dto> InsertAsync(PayloadDto model);
        Task<Dto?> UpdateAsync(Guid id, PayloadDto model);
        Task<bool> DeleteAsync(Guid id);
    }
}
