import { IBaseEntityDto } from './base-entity.dto';

export interface IDevicePayloadDto {
    serialNumber: string;
    uniqueId: string;
    appVersionNumber: string,
    appBuildNumber: string,
    os: string;
    osVersion: string;
    isTV: boolean;
    isTesting: boolean;
    isIOS: boolean;
    isAndroid: boolean;
    isWindows: boolean;
    isMacOS: boolean;
    isWeb: boolean;
}

export interface IDeviceDto extends IDevicePayloadDto, IBaseEntityDto {
    key: string;
    createdTime: Date;
    lastHeartBeat: Date;
}