import { Component, inject, OnInit } from '@angular/core';
import { NotificationsService } from '../../services/notifications.service';
import { AppSettingsService } from '../../services/app-settings.service';

@Component({
  selector: 'app-test',
  standalone: false,
  templateUrl: './test.component.html',
  styleUrl: './test.component.scss'
})
export class TestComponent implements OnInit {
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _appSettingsService = inject(AppSettingsService);

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
