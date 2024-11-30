import { Component } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ExamListFilterComponent } from '../exam-list-filter/exam-list-filter.component';

@Component({
  selector: 'app-exams',
  templateUrl: './exams.component.html',
  styleUrl: './exams.component.scss'
})
export class ExamsComponent {

  constructor(
    private readonly dialog: MatDialog,
  ) { }

  openFilter() {
    const dialogRef = this.dialog.open(ExamListFilterComponent, {
      height: '80vh',
      width: '80vw',
      disableClose: true,
    });

    // dialogRef.afterClosed().subscribe(result => {
    //   console.log('The dialog was closed');
    //   // if (result !== undefined) {
    //   //   // this.animal.set(result);
    //   // }
    // });
  }
}
