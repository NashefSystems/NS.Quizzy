import { Component, inject, NgZone, OnInit } from '@angular/core';
import { NotificationsService } from '../../services/notifications.service';
import { AppSettingsService } from '../../services/app-settings.service';
import { MESSAGE_RESPONSE_EVENTS, MESSAGE_TYPES } from '../../enums/react-native.enum';

@Component({
  selector: 'app-test',
  standalone: false,
  templateUrl: './developer.component.html',
  styleUrl: './developer.component.scss'
})
export class DeveloperComponent implements OnInit {
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _appSettingsService = inject(AppSettingsService);
  private readonly _ngZone = inject(NgZone);
  public readonly reactNativeMessageTypes: { [key: string]: string } = {};

  isLoading = false;

  constructor() {
    this.reactNativeMessageTypes[MESSAGE_TYPES.CONSOLE] = '';
    this.reactNativeMessageTypes[MESSAGE_TYPES.STORE_DATA] = '';
    this.reactNativeMessageTypes[MESSAGE_TYPES.READ_DATA] = MESSAGE_RESPONSE_EVENTS.ON_READ_DATA_RESPONSE;
    this.reactNativeMessageTypes[MESSAGE_TYPES.CHECK_BIOMETRIC_AVAILABILITY] = MESSAGE_RESPONSE_EVENTS.ON_CHECK_BIOMETRIC_AVAILABILITY_RESPONSE;
    this.reactNativeMessageTypes[MESSAGE_TYPES.VERIFY_BIOMETRIC_SIGNATURE] = MESSAGE_RESPONSE_EVENTS.ON_VERIFY_BIOMETRIC_SIGNATURE_RESPONSE;
    this.reactNativeMessageTypes[MESSAGE_TYPES.NOTIFICATION_TOKEN] = MESSAGE_RESPONSE_EVENTS.ON_NOTIFICATION_TOKEN_RESPONSE;
    this.reactNativeMessageTypes[MESSAGE_TYPES.MOBILE_SERIAL_NUMBER] = MESSAGE_RESPONSE_EVENTS.ON_MOBILE_SERIAL_NUMBER_RESPONSE;
  }

  ngOnInit(): void {
    this._appSettingsService.onLoadingStatusChange.subscribe({
      next: (loadingStatus: boolean) => {
        this.isLoading = loadingStatus;
      }
    });
  }

  toggleLoadingMode() {
    this.isLoading = !this.isLoading;
    this._appSettingsService.setLoadingStatus(this.isLoading);
  }

  onReactNativeMessage(type: string) {
    const _window = (window as any);
    if (!_window.ReactNativeWebView) {
      console.log(`[${type}] window.ReactNativeWebView is null`);
      return;
    }

    let data: any = { type: type };
    this.reactNativeMessageTypes
    const responseEvent: string | null = this.reactNativeMessageTypes[type];
    switch (type) {
      case MESSAGE_TYPES.CONSOLE: {
        data.data = ["Is", "Console", "Test"]
        break;
      }
      case MESSAGE_TYPES.STORE_DATA: {
        data.key = "testKey22";
        data.value = {
          nashef: 1,
          mahmoud: 2
        };
        break;
      }
      case MESSAGE_TYPES.READ_DATA: {
        data.key = "testKey22";
        break;
      }
      default: {
        break;
      }
    }

    const eventRNWebView = JSON.stringify(data);
    _window.ReactNativeWebView.postMessage(eventRNWebView);
    if (responseEvent) {
      _window[responseEvent] = (res: any) => {
        console.log("Received:", res);
        this._ngZone.run(() => {
          console.log(`[${type}] response: `, res);
          alert(`[${type}] response: '${JSON.stringify(res)}'`);
        });
      };
    }
  }

  showNotify(type: string) {
    switch (type) {
      case 'info':
        this._notificationsService.info("info");
        break;
      case 'success':
        this._notificationsService.success("success");
        break;
      case 'warning':
        this._notificationsService.warning("warning");
        break;
      case 'error':
        this._notificationsService.error("error");
        break;
      case 'fatal':
        this._notificationsService.fatal('UNEXPECTED_ERROR', { message: "test error" });
        break;
    }
  }
}
