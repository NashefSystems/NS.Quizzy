import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { AppSettingsService } from '../../../services/app-settings.service';
import { AppTranslateService } from '../../../services/app-translate.service';
import { AccountService } from '../../../services/backend/account.service';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';
import { INotificationEvent } from '../../../models/webview-bridge.models';
import { Router } from '@angular/router';
import { DevicesService } from '../../../services/backend/devices.service';
import { IDevicePayloadDto } from '../../../models/backend/device.dto';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './root.component.html',
  styleUrl: './root.component.scss'
})
export class RootComponent implements AfterViewInit, OnInit {
  private readonly _appSettingsService = inject(AppSettingsService);
  private readonly _devicesService = inject(DevicesService);
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _accountService = inject(AccountService);
  private readonly _router = inject(Router);

  userLoggedIn = false;
  isReady = false;
  isLoading = false;

  appContainerClasses = {
    "app-container": true,
    "large-screen": false
  };

  ngOnInit(): void {
    this.onResize();

    this._appSettingsService.onLoadingStatusChange.subscribe({
      next: (loadingStatus) => {
        this.isLoading = loadingStatus;
      }
    });

    this._appTranslateService.onDirectionChange.subscribe({
      next: (dir) => {
        document.documentElement.style.setProperty("--app-dir", dir);
        document.documentElement.setAttribute("dir", dir);
        this.isReady = !!dir;
      }
    });

    this._accountService.userChange.subscribe(user => this.userLoggedIn = !!(user?.id));


    this.runAppTasks();
  }

  runAppTasks() {
    if (!this._webviewBridgeService.nativeAppIsAvailable()) {
      console.info("runAppTasksAsync | native app is not available");
      return;
    }

    this.onPushNotificationReceived();
    this.updateDeviceInfoAsync();
  }

  async updateDeviceInfoAsync() {
    try {
      console.info('updateDeviceInfoAsync | Starting');
      const mobileSerialNumber = await this._webviewBridgeService.getMobileSerialNumberAsync();
      const platformInfo = await this._webviewBridgeService.getPlatformInfoAsync();
      if (!mobileSerialNumber) {
        console.error('updateDeviceInfoAsync | mobileSerialNumber is null, ', mobileSerialNumber);
        return;
      }
      if (!platformInfo) {
        console.error('updateDeviceInfoAsync | platformInfo is null, ', platformInfo);
        return;
      }

      const request: IDevicePayloadDto = {
        serialNumber: mobileSerialNumber.serialNumber ?? '',
        uniqueId: mobileSerialNumber.uniqueId ?? '',
        appVersionName: platformInfo.appVersionName,
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

  onPushNotificationReceived() {
    const _window = window as any;
    const eventType = this._appSettingsService.PUSH_NOTIFICATION_IS_RECEIVED_EVENT_TYPE;
    const listener = (notificationEvent: INotificationEvent) => {
      try {
        console.log("event listener:\n[notificationEvent] ", notificationEvent);
        this._router.navigate(['/my-notifications']);
      } catch (err: any) {
        console.error("onPushNotificationReceived | Listener exception: ", err);
      }
    };
    _window[eventType] = listener;
  }

  ngAfterViewInit(): void {
    window.addEventListener('resize', this.onResize.bind(this));
  }

  onResize() {
    const isMobileApp = this._webviewBridgeService.nativeAppIsAvailable();
    const isLargeScreenMode = (window.innerWidth > this._appSettingsService.appMaxWidth) && !isMobileApp;
    this._appSettingsService.setLargeScreenMode(isLargeScreenMode);
    document.documentElement.style.setProperty("--is-large-screen", isLargeScreenMode ? 'true' : 'false');
    this.appContainerClasses['large-screen'] = isLargeScreenMode;

    const maxHeight = window.innerHeight * (isLargeScreenMode ? 0.8 : 1);
    this._appSettingsService.setAppMaxHeight(maxHeight);
    document.documentElement.style.setProperty("--app-max-height", maxHeight + "px");

    const maxWidth = isLargeScreenMode ? this._appSettingsService.appMaxWidth : window.innerWidth;
    this._appSettingsService.setAppMaxWidth(maxWidth);
    document.documentElement.style.setProperty("--app-max-width", maxWidth + "px");
  }
}
