import { inject, Injectable } from '@angular/core';
import { DevicesService } from './backend/devices.service';
import { WebviewBridgeService } from './webview-bridge.service';
import { IDevicePayloadDto } from '../models/backend/device.dto';
import { IDownloadFilePayload, IGetMobileSerialNumberResponse, IGetPlatformInfoResponse } from '../models/webview-bridge.models';
import { DownloadFileUtils } from '../utils/download-file.utils';
import { FeatureFlags } from '../enums/feature-flags.enum';
import { ClientAppSettingsService } from './backend/client-app-settings.service';
import { IUpdateAppCheckResponse } from '../models/check-update-is-nedded.response';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  private readonly _devicesService = inject(DevicesService);
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  private static _platformInfoResponse: IGetPlatformInfoResponse | null = null;
  private static _mobileSerialNumberResponse: IGetMobileSerialNumberResponse | null = null;
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);


  constructor() {
    this._webviewBridgeService.getPlatformInfoAsync().then(x => GlobalService._platformInfoResponse = x);
  }

  async updateAppCheck() {
    console.info('updateAppCheck | Starting');

    const response: IUpdateAppCheckResponse = {
      updateRequired: false
    };

    const nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
    if (!nativeAppIsAvailable) {
      console.error('updateAppCheck | native app is not available');
      return response;
    }

    if (!GlobalService._platformInfoResponse) {
      GlobalService._platformInfoResponse = await this._webviewBridgeService.getPlatformInfoAsync();
    }

    const platformInfo = GlobalService._platformInfoResponse;
    if (!platformInfo) {
      console.error('updateAppCheck | platformInfo is null, ', platformInfo);
      return response;
    }

    const clientAppSettings = await this._clientAppSettingsService.get().toPromise();
    if (!clientAppSettings) {
      console.error('updateAppCheck | clientAppSettings is null, ', platformInfo);
      return response;
    }

    const currentAppBuildNumber = +platformInfo.appBuildNumber;
    const minAppBuildNumber = platformInfo.os === 'android' ? clientAppSettings.MinAppBuildNumberAndroid : clientAppSettings.MinAppBuildNumberIOS;
    console.info(`updateAppCheck | currentAppBuildNumber: '${currentAppBuildNumber}', OS: '${platformInfo.os}', minAppBuildNumber: '${minAppBuildNumber}'`);
    response.updateRequired = currentAppBuildNumber < minAppBuildNumber;
    if (response.updateRequired) {
      if (platformInfo.os === 'android') {
        response.platform = 'android';
        response.storeURL = clientAppSettings.StoreUrlAndroid;
      } else {
        response.platform = 'ios';
        response.storeURL = clientAppSettings.StoreUrlIOS;
      }
    }
    console.info('updateAppCheck | response: ', response);
    return response;
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

  async featureIsAvailableAsync(featureFlag: FeatureFlags) {
    switch (featureFlag) {
      case FeatureFlags.EXAM_SCHEDULE_LIST__DOWNLOAD_FILE:
      case FeatureFlags.USER_LIST__DOWNLOAD_FILE:
        {
          const nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
          if (!nativeAppIsAvailable) {
            return true;
          }
          const platformInfo = await this._webviewBridgeService.getPlatformInfoAsync();
          const buildNumber = !platformInfo ? 0 : +platformInfo.appBuildNumber;
          return buildNumber >= 1000000040; // v1.0.40
        }
      default:
        return false;
    }
  }
}
