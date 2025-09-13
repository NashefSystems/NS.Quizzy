using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NS.Quizzy.Server.BL.CustomExceptions;
using NS.Quizzy.Server.BL.Interfaces;
using NS.Quizzy.Server.DAL;
using NS.Quizzy.Server.BL.DTOs;
using NS.Quizzy.Server.BL.Utils;

namespace NS.Quizzy.Server.BL.Services
{
    internal class DevicesService : IDevicesService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;

        public DevicesService(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
        }

        public async Task<DeviceDto> UpdateInfoAsync(DevicePayloadDto payload)
        {
            if (payload == null)
            {
                throw new BadRequestException("Payload is null");
            }

            if (string.IsNullOrWhiteSpace(payload.UniqueId) && string.IsNullOrWhiteSpace(payload.SerialNumber))
            {
                throw new BadRequestException($"Either '{nameof(payload.UniqueId)}' or '{nameof(payload.SerialNumber)}' must be provided");
            }

            var startActionTime = DateTimeOffset.Now;

            var id = (StringUtils.FirstNotNullOrWhiteSpace(
                    payload.UniqueId,
                    payload.SerialNumber
                ) ?? string.Empty)
                .ToUpper()
                .Trim();
            payload.AppVersion ??= string.Empty;
            var item = await _appDbContext.Devices.FirstOrDefaultAsync(x => x.ID == id && x.AppVersion == payload.AppVersion);
            if (item == null)
            {
                item = new DAL.Entities.Device()
                {
                    ID = id,
                    AppBuildNumber = payload.AppBuildNumber,
                    AppVersion = payload.AppVersion,
                    SerialNumber = payload.SerialNumber,
                    UniqueId = payload.UniqueId,
                    OS = payload.OS,
                    IsTV = payload.IsTV,
                    IsTesting = payload.IsTesting,
                    IsIOS = payload.IsIOS,
                    IsAndroid = payload.IsAndroid,
                    IsWindows = payload.IsWindows,
                    IsMacOS = payload.IsMacOS,
                    IsWeb = payload.IsWeb,
                    CreatedTime = startActionTime,
                };
                await _appDbContext.Devices.AddAsync(item);
            }

            item.OSVersion = payload.OSVersion;
            item.AppVersion = payload.AppVersion ?? string.Empty;
            item.AppBuildNumber = payload.AppBuildNumber;
            item.LastHeartBeat = startActionTime;
            await _appDbContext.SaveChangesAsync();

            return _mapper.Map<DeviceDto>(item);
        }
    }
}
