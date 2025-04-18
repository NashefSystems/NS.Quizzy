using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Extensions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.BL.Models;
using NS.Quizzy.Server.Common.Extensions;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.DAL.Entities;
using NS.Quizzy.Server.BL.DTOs;
using NS.Shared.CacheProvider.Interfaces;
using NS.Shared.Logging;
using NS.Shared.QueueManager.Interfaces;
using NS.Shared.QueueManager.Models;
using System.Text;
using static NS.Quizzy.Server.Common.Enums;
using static NS.Quizzy.Server.DAL.DALEnums;

namespace NS.Quizzy.Server.BL.Services
{
    internal class UsersService : IUsersService
    {
        const string CACHE_KEY = "Users";
        private readonly INSQueueService _queueService;
        private readonly INSCacheProvider _cacheProvider;
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly TimeSpan _cacheDataTTL;
        private readonly string _idNumberEmailDomain;
        private readonly Roles[] _roles;
        private readonly INSLogger _logger;


        public UsersService(INSLogger logger, AppDbContext appDbContext, INSCacheProvider cacheProvider, IMapper mapper, IConfiguration configuration, INSQueueService queueService)
        {
            _logger = logger;
            _appDbContext = appDbContext;
            _cacheProvider = cacheProvider;
            _mapper = mapper;
            _queueService = queueService;
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

        public async Task ClearCacheAsync()
        {
            await _cacheProvider.DeleteAsync(CACHE_KEY);
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
                IdNumber = model.IdNumber,
                FullName = model.FullName,
                ClassId = model.ClassId,
                Role = model.Role,
            };
            await _appDbContext.Users.AddAsync(item);
            await _appDbContext.SaveChangesAsync();
            await ClearCacheAsync();

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
            item.IdNumber = model.IdNumber;
            item.ClassId = model.ClassId;
            item.Role = model.Role;

            await _appDbContext.SaveChangesAsync();
            await ClearCacheAsync();

            return _mapper.Map<UserDto>(item);
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
            await ClearCacheAsync();
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
                var role = item.Role.ToHebrewRole();
                var classCode = item.Class?.GetFullCode();
                var idNumber = item.Email.Replace($"@{_idNumberEmailDomain}", "");
                sb.AppendLine($"\"{idNumber}\",\"{item.FullName}\",\"{role}\",{classCode}");
            }

            var csvContent = sb.ToString();
            var utf8Bytes = Encoding.UTF8.GetBytes(csvContent);
            var utf8WithBom = Encoding.UTF8.GetPreamble().Concat(utf8Bytes).ToArray();
            return utf8WithBom;
        }

        public async Task<UploadFileResponse> UploadAsync(IFormFile file)
        {
            using var logBag = _logger.CreateLogBag(nameof(UploadAsync));
            if (file == null || file.Length == 0)
            {
                logBag.Trace("No file uploaded");
                logBag.LogLevel = NSLogLevel.Error;
                throw new BadRequestException("No file uploaded");
            }

            var allowedExtensions = new[] { ".csv" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
            {
                logBag.Trace("Invalid file type. Only CSV files are allowed");
                logBag.LogLevel = NSLogLevel.Error;
                throw new BadRequestException("Invalid file type. Only CSV files are allowed");
            }

            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            string csvContent = await reader.ReadToEndAsync();

            // Validate if the file is empty after reading
            if (string.IsNullOrWhiteSpace(csvContent))
            {
                logBag.Trace("CSV file is empty");
                logBag.LogLevel = NSLogLevel.Warn;
                throw new BadRequestException("CSV file is empty");
            }

            var (items, errors) = CsvFileValidatorAndExtractor(csvContent);
            if (errors.Count != 0)
            {
                var errMsg = string.Join('\n', errors);
                logBag.Trace($"Error message:\n{errMsg}");
                logBag.LogLevel = NSLogLevel.Error;
                throw new BadRequestException(errMsg);
            }

            var publishMessageResult = await _queueService.PublishMessageAsync(new Shared.QueueManager.Models.NSQueueMessage()
            {
                QueueName = BLConsts.QUEUE_UPDATE_USERS,
                Payload = JsonConvert.SerializeObject(items)
            });

            logBag.AddOrUpdateParameter("PublishMessageResult", publishMessageResult);

            if (!publishMessageResult.IsSuccessful)
            {
                throw new Exception($"Unable to push queue message: '{publishMessageResult.Error}'");
            }

            return new UploadFileResponse()
            {
                MessageId = publishMessageResult.MessageID
            };
        }

        public async Task<UploadFileStatusResponse> UploadFileStatusAsync(Guid uploadMessageId)
        {
            var messageInfo = await _queueService.GetMessageStatusInfoAsync(uploadMessageId);

            return new UploadFileStatusResponse()
            {
                IsCompleted = messageInfo.IsCompleted,
                ProgressPercentage = messageInfo.ProgressPercentage
            };
        }

        private (List<CsvFileItem> items, List<string> errors) CsvFileValidatorAndExtractor(string csvContent)
        {
            using var logBag = _logger.CreateLogBag(nameof(CsvFileValidatorAndExtractor));
            logBag.AddOrUpdateParameter("CsvContentLength", csvContent?.Length);
            var items = new List<CsvFileItem>();
            var errors = new List<string>();

            var lines = csvContent
                .Split(['\r', '\n'])
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            logBag.AddOrUpdateParameter("LinesLength", lines?.Length);

            for (int i = 0; i < lines.Length; i++)
            {
                var csvItem = ToCsvFileItem(lines[i]);
                List<string> lineErrors;
                if (i == 0) // header row
                {

                    if (csvItem.IdNumber != "תעודת זהות" || csvItem.FullName != "שם מלא" || csvItem.Role != "תפקיד" || csvItem.Class != "כיתה")
                    {
                        lineErrors = [
                            "Invalid header row"
                        ];
                    }
                    continue;
                }
                else
                {
                    lineErrors = GetCsvFileItemErrors(csvItem);
                }

                if (lineErrors.Count > 0)
                {
                    errors.Add($"({i + 1}) {string.Join(", ", lineErrors)}");
                }
                else if (lineErrors.Count == 0 && i != 0)
                {
                    items.Add(csvItem);
                }
            }
            logBag.AddOrUpdateParameter("ItemsLength", items.Count);
            logBag.AddOrUpdateParameter("ErrorsLength", errors.Count);

            if (errors.Count != 0)
            {
                logBag.LogLevel = NSLogLevel.Error;
            }

            return (items, errors);
        }

        private static CsvFileItem ToCsvFileItem(string line)
        {
            var res = new CsvFileItem();

            //"תעודת זהות,שם מלא,תפקיד,כיתה"
            var lineParts = line.Split(',', StringSplitOptions.TrimEntries);
            if (lineParts.Length >= 4)
            {
                res.IdNumber = lineParts[0].Trim().PadLeft(9, '0');
                res.FullName = lineParts[1].Trim();
                res.Role = lineParts[2].Trim();
                res.Class = lineParts[3].Trim();
            }

            return res;
        }

        private static List<string> GetCsvFileItemErrors(CsvFileItem item)
        {
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(item.IdNumber))
            {
                errors.Add("Invalid ID number");
            }

            if (string.IsNullOrWhiteSpace(item.FullName))
            {
                errors.Add("Invalid full name");
            }

            if (item.Role != "תלמיד" && item.Role != "מורה")
            {
                errors.Add("Invalid role");
            }

            if (item.Role == "תלמיד" && !(int.TryParse(item.Class, out int classNo) && classNo >= 1001 && classNo <= 1411))
            {
                errors.Add("Invalid class number");
            }

            return errors;
        }
    }
}
