import { Component, inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { INotificationTemplateDto } from '../../../models/backend/notification-template.dto';

@Component({
  selector: 'app-select-notification-template-dialog',
  standalone: false,
  templateUrl: './select-notification-template-dialog.component.html',
  styleUrl: './select-notification-template-dialog.component.scss'
})
export class SelectNotificationTemplateDialogComponent {
  private readonly dialogRef = inject(MatDialogRef<SelectNotificationTemplateDialogComponent>);
  readonly items = inject<INotificationTemplateDto[]>(MAT_DIALOG_DATA);
  selectedItem: INotificationTemplateDto | null = null;

  confirm() {
    this.dialogRef.close(this.selectedItem);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
