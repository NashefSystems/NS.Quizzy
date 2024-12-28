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
    let width = window.innerWidth;
    let height = window.innerHeight;

    if (width > this.appSettingsService.appMaxWidth) {
      width = this.appSettingsService.appMaxWidth;
      height *= 0.8;
    }

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
        dialogConfig.panelClass.push(this.appSettingsService.isLargeScreen() ?
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
