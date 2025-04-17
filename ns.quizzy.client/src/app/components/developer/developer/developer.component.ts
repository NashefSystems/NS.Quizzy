import { Component, inject, NgZone, OnInit } from '@angular/core';
import { NotificationsService } from '../../../services/notifications.service';
import { AppSettingsService } from '../../../services/app-settings.service';

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
      next: (loadingStatus: boolean) => {
        this.isLoading = loadingStatus;
      }
    });
  }

  toggleLoadingMode() {
    this.isLoading = !this.isLoading;
    this._appSettingsService.setLoadingStatus(this.isLoading);
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
