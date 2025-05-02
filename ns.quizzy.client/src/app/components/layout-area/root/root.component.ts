import { AfterViewInit, Component, inject, OnInit } from '@angular/core';
import { AppSettingsService } from '../../../services/app-settings.service';
import { AppTranslateService } from '../../../services/app-translate.service';
import { AccountService } from '../../../services/backend/account.service';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';
import { INotificationEvent } from '../../../models/webview-bridge.models';
import { Router } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: false,
  templateUrl: './root.component.html',
  styleUrl: './root.component.scss'
})
export class RootComponent implements AfterViewInit, OnInit {
  private readonly _appSettingsService = inject(AppSettingsService);
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


    this.onPushNotificationReceived();
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
