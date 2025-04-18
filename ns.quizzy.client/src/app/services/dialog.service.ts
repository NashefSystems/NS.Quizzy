import { inject, Injectable, Renderer2, RendererFactory2 } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { AppSettingsService } from './app-settings.service';
import { OpenDialogPayload } from '../models/dialog/open-dialog.payload';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private readonly dialog = inject(MatDialog);
  private readonly appSettingsService = inject(AppSettingsService);

  openDialog(payload: OpenDialogPayload) {
    let width = this.appSettingsService.getAppMaxWidth();
    let height = this.appSettingsService.getAppMaxHeight();

    //#region set default values
    if (payload.disableClose === undefined) payload.disableClose = true;
    if (payload.isFullDialog === undefined) payload.isFullDialog = false;
    //#endregion

    const maxWidthRation = 0.9;
    const maxHeightRation = 0.9;

    return new Promise((resolve, reject) => {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = payload.disableClose;
      dialogConfig.autoFocus = true;
      dialogConfig.hasBackdrop = true;
      dialogConfig.panelClass = ['app-dialog'];

      if (payload.isFullDialog) {
        dialogConfig.minWidth = dialogConfig.maxWidth = width;
        dialogConfig.minHeight = dialogConfig.maxHeight = height;
        dialogConfig.panelClass.push('full-dialog');
        dialogConfig.panelClass.push(this.appSettingsService.isLargeScreenMode() ?
          'full-dialog-large-screen' : 'mat-dialog-border-without-radius');
      } else {
        dialogConfig.maxWidth = width * maxWidthRation;
        dialogConfig.maxHeight = height * maxHeightRation;
      }

      document.documentElement.style.setProperty("--app-dialog-max-width", dialogConfig.maxWidth + "px");
      document.documentElement.style.setProperty("--app-dialog-max-height", dialogConfig.maxHeight + "px");
      dialogConfig.data = payload.data;

      this.dialog
        .open(payload.component, dialogConfig)
        .afterClosed()
        .subscribe(result => {
          resolve(result);
        });
    });
  }
}
