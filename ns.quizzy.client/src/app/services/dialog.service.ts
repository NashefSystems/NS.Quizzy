import { ComponentType } from '@angular/cdk/portal';
import { inject, Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { AppSettingsService } from './app-settings.service';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private readonly dialog = inject(MatDialog);
  private readonly appSettingsService = inject(AppSettingsService);

  public openDialog(component: ComponentType<any>, data: any, disableClose: boolean = true) {
    let width = window.innerWidth;
    let height = window.innerHeight;

    if (width > this.appSettingsService.appMaxWidth) {
      width = this.appSettingsService.appMaxWidth;
      height *= 0.8;
    }

    return new Promise((resolve, reject) => {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = disableClose;
      dialogConfig.autoFocus = true;
      dialogConfig.minWidth = width * 0.9;
      dialogConfig.maxWidth = width * 0.9;
      dialogConfig.maxHeight = height * 0.9;
      dialogConfig.data = data;

      this.dialog
        .open(component, dialogConfig)
        .afterClosed()
        .subscribe(result => {
          resolve(result);
        });
    });
  }
}
