import { ComponentType } from '@angular/cdk/portal';
import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { ExamListFilterComponent } from '../coponents/exam-list-filter/exam-list-filter.component';

@Injectable({
  providedIn: 'root'
})
export class DialogService {
  private readonly DIALOG_MIN_WIDTH = "50vw";
  private readonly DIALOG_MAX_WIDTH = "80vw";
  private readonly DIALOG_MAX_HEIGHT = "80vh";

  constructor(
    private dialog: MatDialog
  ) { }

  openExamListFilterDialog() {
    return this.openDialog(ExamListFilterComponent, null, true);
  }

  private openDialog(component: ComponentType<any>, data: any, disableClose: boolean = true, minWidth: string | null = null) {
    return new Promise((resolve, reject) => {
      const dialogConfig = new MatDialogConfig();
      dialogConfig.disableClose = disableClose;
      dialogConfig.autoFocus = true;
      dialogConfig.minWidth = minWidth ?? this.DIALOG_MIN_WIDTH;
      dialogConfig.maxWidth = this.DIALOG_MAX_WIDTH;
      dialogConfig.maxHeight = this.DIALOG_MAX_HEIGHT;
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
