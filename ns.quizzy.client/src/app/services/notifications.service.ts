import { inject, Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material/snack-bar';
import { NotificationSnackBarComponent } from '../components/global-area/notification-snack-bar/notification-snack-bar.component';
import { INotificationSnackBarData, NotificationSnackBarType } from '../components/global-area/notification-snack-bar/notification-snack-bar.data';
import { AppTranslateService } from './app-translate.service';

@Injectable({
    providedIn: 'root'
})
export class NotificationsService {
    private _snackBar = inject(MatSnackBar);
    private _appTranslateService = inject(AppTranslateService);

    info(message: string, messageParams: { [key: string]: any } = {}, duration: number = 3000) {
        this.notify(message, messageParams, 'info', duration);
    }

    success(message: string, messageParams: { [key: string]: any } = {}, duration: number = 3000) {
        this.notify(message, messageParams, 'success', duration);
    }

    warning(message: string, messageParams: { [key: string]: any } = {}, duration: number = 3000) {
        this.notify(message, messageParams, 'warning', duration);
    }

    error(message: string, messageParams: { [key: string]: any } = {}, duration: number = 3000) {
        this.notify(message, messageParams, 'error', duration);
    }

    fatal(message: string, messageParams: { [key: string]: any } = {}, duration: number = 3000) {
        this.notify(message, messageParams, 'fatal', duration);
    }

    httpErrorHandler(err: any) {
        const msg = err?.error?.message || err?.message;
        if (err?.status < 500) {
            this.error(this._appTranslateService.translate(msg));
            return;
        }
        console.error(err);
        this.fatal('UNEXPECTED_ERROR', { message: this._appTranslateService.translate(msg) });
    }

    private notify(message: string, messageParams: { [key: string]: any } = {}, type: NotificationSnackBarType, duration: number = 3000): void {
        const data: INotificationSnackBarData = {
            message: this._appTranslateService.translate(message, messageParams).replaceAll("\n", "<br>"),
            type: type
        };
        const config: MatSnackBarConfig = {
            direction: this._appTranslateService.isRtlDirection() ? 'rtl' : 'ltr',
            duration: duration,
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
            panelClass: ['no-panel', `snack-bar-${type}`],
            data: data,
        }
        this._snackBar.openFromComponent(NotificationSnackBarComponent, config);
    }
}
