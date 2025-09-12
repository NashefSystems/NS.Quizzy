import { inject, Injectable } from '@angular/core';
import { DevicesService } from './backend/devices.service';
import { WebviewBridgeService } from './webview-bridge.service';
import { IDevicePayloadDto } from '../models/backend/device.dto';
import { IGetMobileSerialNumberResponse, IGetPlatformInfoResponse } from '../models/webview-bridge.models';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  private readonly _devicesService = inject(DevicesService);
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  private static _platformInfoResponse: IGetPlatformInfoResponse | null = null;
  private static _mobileSerialNumberResponse: IGetMobileSerialNumberResponse | null = null;

  constructor() {
    this._webviewBridgeService.getPlatformInfoAsync().then(x => GlobalService._platformInfoResponse = x);
  }

  async updateDeviceInfoAsync() {
    try {
      console.info('updateDeviceInfoAsync | Starting');
      const nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
      if (!nativeAppIsAvailable) {
        console.error('updateDeviceInfoAsync | native app is not available');
        return;
      }

      if (!GlobalService._mobileSerialNumberResponse) {
        GlobalService._mobileSerialNumberResponse = await this._webviewBridgeService.getMobileSerialNumberAsync();
      }
      const mobileSerialNumber = GlobalService._mobileSerialNumberResponse;

      if (!mobileSerialNumber) {
        console.error('updateDeviceInfoAsync | mobileSerialNumber is null, ', mobileSerialNumber);
        return;
      }

      if (!GlobalService._platformInfoResponse) {
        GlobalService._platformInfoResponse = await this._webviewBridgeService.getPlatformInfoAsync();
      }
      const platformInfo = GlobalService._platformInfoResponse;

      if (!platformInfo) {
        console.error('updateDeviceInfoAsync | platformInfo is null, ', platformInfo);
        return;
      }

      const request: IDevicePayloadDto = {
        serialNumber: mobileSerialNumber.serialNumber ?? '',
        uniqueId: mobileSerialNumber.uniqueId ?? '',
        appVersion: platformInfo.appVersion,
        appBuildNumber: platformInfo.appBuildNumber,
        os: platformInfo.os,
        osVersion: platformInfo.version.toString(),
        isTV: platformInfo.isTV,
        isTesting: platformInfo.isTesting,
        isIOS: platformInfo.isIOS,
        isAndroid: platformInfo.isAndroid,
        isWindows: platformInfo.isWindows,
        isMacOS: platformInfo.isMacOS,
        isWeb: platformInfo.isWeb,
      };
      this._devicesService.updateInfoAsync(request)
        .subscribe(x => console.info('updateDeviceInfoAsync | response: ', x));
    }
    catch (err: any) {
      console.error('updateDeviceInfoAsync | Exception: ', err);
    }
  }

  async getAppVersion() {
    const nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
    if (nativeAppIsAvailable && !GlobalService._platformInfoResponse) {
      GlobalService._platformInfoResponse = await this._webviewBridgeService.getPlatformInfoAsync();
    }
    return GlobalService._platformInfoResponse?.appVersion || null;
  }
}
