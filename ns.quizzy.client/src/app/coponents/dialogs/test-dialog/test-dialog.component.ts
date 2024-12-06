import { Component, inject } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-test-dialog',
  templateUrl: './test-dialog.component.html',
  styleUrl: './test-dialog.component.scss'
})
export class TestDialogComponent {
  readonly dialogRef = inject(MatDialogRef<TestDialogComponent>);
  //readonly data = inject<DialogData>(MAT_DIALOG_DATA);
  //readonly animal = model(this.data.animal);

  onCloseClick(): void {
    this.dialogRef.close();
  }
}
