import { inject, Injectable } from '@angular/core';
import { DevicesService } from './backend/devices.service';
import { WebviewBridgeService } from './webview-bridge.service';
import { IDevicePayloadDto } from '../models/backend/device.dto';
import { IDownloadFilePayload, IGetMobileSerialNumberResponse, IGetPlatformInfoResponse, IOpenURLPayload } from '../models/webview-bridge.models';
import { DownloadFileUtils } from '../utils/download-file.utils';

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

  async downloadFileAsync(base64: string, fileName: string, mimeType: string) {
    let url: string = ''
    try {
      const appIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
      console.info(`downloadFile | appIsAvailable: '${appIsAvailable}', fileName: '${fileName}', mimeType: '${mimeType}'`);
      if (appIsAvailable) {
        const payload: IDownloadFilePayload = {
          base64,
          fileName,
          mimeType
        };
        const res = await this._webviewBridgeService.downloadFileAsync(payload);
        console.info(`downloadFile | WebView | res: `, res);
      } else {
        const blob = DownloadFileUtils.base64ToBlob(base64, mimeType);
        DownloadFileUtils.downloadBlobFile(blob, fileName);
        console.info(`downloadFile | local download`);
      }
    } catch (err) {
      console.error(`downloadFile | Exception: `, err);
    }
  }
}
