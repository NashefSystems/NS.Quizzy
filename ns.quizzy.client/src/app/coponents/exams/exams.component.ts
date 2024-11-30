import { Component } from '@angular/core';
import { ExamListFilterComponent } from '../exam-list-filter/exam-list-filter.component';
import { DialogService } from '../../services/dialog.service';

@Component({
  selector: 'app-exams',
  templateUrl: './exams.component.html',
  styleUrl: './exams.component.scss'
})
export class ExamsComponent {

  constructor(
    private readonly dialogService: DialogService,
  ) { }

  openFilter() {
    const dialogRef = this.dialogService.openExamListFilterDialog()
      .then(result => {
        console.log('The dialog was closed');
        if (result !== undefined) {
          // this.animal.set(result);
        }
      });
  }
}
