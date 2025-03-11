using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.Models.DTOs;
using NS.Shared.CacheProvider.Interfaces;
using System.Text;
using static NS.Quizzy.Server.Common.Enums;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class UsersService : IUsersService
    {
        const string CACHE_KEY = "Users";
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;
        private readonly string _idNumberEmailDomain;
        private readonly Roles[] _roles;


        public UsersService(AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration)
        {
            _appDbContext = appDbContext;
            _cacheProvider = cacheProvider;
            _mapper = mapper;
            _roles = [Roles.Teacher, Roles.Student];
            {
                var key = AppSettingKeys.CacheDataTTLMin.GetDBStringValue();
                var valueInMin = double.TryParse(configuration.GetValue<string>(key), out double val) ? val : 60;
                _cacheDataTTL = TimeSpan.FromMinutes(valueInMin);
            }
            {
                var key = AppSettingKeys.IdNumberEmailDomain.GetDBStringValue();
                _idNumberEmailDomain = configuration.GetValue<string>(key) ?? "";
            }
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var cacheValue = await _cacheProvider.GetAsync<List<UserDto>>(CACHE_KEY);
            if (cacheValue != null)
            {
                return cacheValue;
            }

            var items = await _appDbContext.Users
                .Where(x => x.IsDeleted == false && _roles.Contains(x.Role))
                .OrderBy(x => x.FullName)
                .ToListAsync();
            var res = _mapper.Map<List<UserDto>>(items);
            await _cacheProvider.SetOrUpdateAsync(CACHE_KEY, res, _cacheDataTTL);
            return res;
        }

        public async Task<UserDto?> GetAsync(Guid id)
        {
            var item = await _appDbContext.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id && _roles.Contains(x.Role));
            if (item == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(item);
        }

        public async Task<UserDto> InsertAsync(UserPayloadDto model)
        {
            if (!_roles.Contains(model.Role))
            {
                throw new ForbiddenException("You do not have permission to insert this role");
            }

            var exists = await _appDbContext.Users.AnyAsync(x => x.IsDeleted == false && x.Email.ToUpper() == model.Email.ToUpper());
            if (exists)
            {
                throw new ConflictException("User already exists");
            }

            var item = new DAL.Entities.User()
            {
                Email = model.Email,
                Password = model.Email.ToLower(),
                FullName = model.FullName,
                ClassId = model.ClassId,
                Role = model.Role,
            };
            await _appDbContext.Users.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<UserDto>(item);
        }

        public async Task<UserDto?> UpdateAsync(Guid id, UserPayloadDto model)
        {
            if (!_roles.Contains(model.Role))
            {
                throw new ForbiddenException("You do not have permission to edit this role");
            }

            var item = await _appDbContext.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id && _roles.Contains(x.Role));
            if (item == null)
            {
                return null;
            }

            if (await _appDbContext.Users.AnyAsync(x => x.IsDeleted == false && x.Id != id && x.Email.ToUpper() == model.Email.ToUpper()))
            {
                throw new ConflictException("There is another user with the same email");
            }

            item.Email = model.Email;
            item.Password = model.Email.ToLower();
            item.FullName = model.FullName;
            item.ClassId = model.ClassId;
            item.Role = model.Role;

            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);

            return _mapper.Map<UserDto>(item);
        }

        private static uint? GetFullCode(Class? c)
        {
            if (c == null || c.Grade == null)
                return null;

            return (c.Grade.Code * 100) + c.Code;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var item = await _appDbContext.Users.FirstOrDefaultAsync(x => x.IsDeleted == false && x.Id == id);
            if (item == null)
            {
                return false;
            }

            if (!_roles.Contains(item.Role))
            {
                throw new ForbiddenException("You do not have permission to delete this role");
            }

            item.IsDeleted = true;
            await _appDbContext.SaveChangesAsync();
            await _cacheProvider.DeleteAsync(CACHE_KEY);
            return true;
        }

        public async Task<byte[]> DownloadAsync()
        {
            var items = await _appDbContext.Users
                .Where(x => x.IsDeleted == false && _roles.Contains(x.Role))
                .Include(x => x.Class)
                .ThenInclude(x => x.Grade)
                .OrderBy(x => x.FullName)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("תעודת זהות,שם מלא,תפקיד,כיתה");

            foreach (var item in items)
            {
                var role = item.Role == Roles.Student ? "תלמיד" : "מורה";
                var classCode = GetFullCode(item.Class);
                var idNumber = item.Email.Replace($"@{_idNumberEmailDomain}", "");
                sb.AppendLine($"\"{idNumber}\",\"{item.FullName}\",\"{role}\",{classCode}");
            }

            var csvContent = sb.ToString();
            var utf8Bytes = Encoding.UTF8.GetBytes(csvContent);
            var utf8WithBom = Encoding.UTF8.GetPreamble().Concat(utf8Bytes).ToArray();
            return utf8WithBom;
        }
    }
}
