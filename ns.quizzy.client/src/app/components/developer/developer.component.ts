import { Component, inject, NgZone, OnInit } from '@angular/core';
import { NotificationsService } from '../../services/notifications.service';
import { AppSettingsService } from '../../services/app-settings.service';

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

  isLoading = false;

  ngOnInit(): void {
    this._appSettingsService.onLoadingStatusChange.subscribe({
      next: (loadingStatus) => {
        this.isLoading = loadingStatus;
      }
    });
  }

  toggleLoadingMode() {
    this.isLoading = !this.isLoading;
    this._appSettingsService.setLoadingStatus(this.isLoading);
  }

  reactNativeMessageKeys = ['test', 'console', 'checkBiometric', 'checkMainUser'];
  onReactNativeMessage(type: string) {
    const _window = (window as any);
    if (!_window.ReactNativeWebView) {
      console.log(`[${type}] window.ReactNativeWebView is null`);
      return;
    }

    let data: any = { type: type };
    let responseEvent: string = "";
    switch (type) {
      case 'test': {
        responseEvent = '';
        break;
      }
      case 'console': {
        data.data = ["Is", "Console", "Test"]
        responseEvent = '';
        break;
      }
      case 'checkBiometric': {
        responseEvent = 'onCheckBiometricResponse';
        break;
      }
      case 'checkMainUser': {
        responseEvent = 'onCheckMainUserResponse';
        data.uid = "uidvalue"
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
