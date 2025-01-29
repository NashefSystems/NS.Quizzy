import { Component, inject, Inject } from '@angular/core';
import { MAT_SNACK_BAR_DATA, MatSnackBarRef } from '@angular/material/snack-bar';
import { INotificationSnackBarData } from './notification-snack-bar.data';

@Component({
  selector: 'app-notification-snack-bar',
  standalone: false,
  templateUrl: './notification-snack-bar.component.html',
  styleUrl: './notification-snack-bar.component.scss'
})
export class NotificationSnackBarComponent {
  private _snackBarRef = inject(MatSnackBarRef<NotificationSnackBarComponent>);
  public data: INotificationSnackBarData = inject(MAT_SNACK_BAR_DATA);

  closeSnackBar(): void {
    this._snackBarRef.dismiss();
  }

  getIcon(type: string): string {
    const icons: { [key: string]: string } = {
      info: 'ℹ️',
      success: '✅',
      warning: '⚠️',
      error: '❌',
      fatal: '🛑',
    };
    return icons[type] || 'ℹ️';
  }
}
