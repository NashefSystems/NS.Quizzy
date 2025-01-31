import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ExamScheduleFilterData, FilterResult } from './exam-schedule-filter.data';

@Component({
  selector: 'app-exam-schedule-filter',
  standalone: false,

  templateUrl: './exam-schedule-filter.component.html',
  styleUrl: './exam-schedule-filter.component.scss'
})
export class ExamScheduleFilterComponent {
  private readonly _fb = inject(FormBuilder);
  private readonly dialogRef = inject(MatDialogRef<ExamScheduleFilterComponent>);
  readonly data = inject<ExamScheduleFilterData>(MAT_DIALOG_DATA);

  form: FormGroup = this._fb.group({
    fromDate: [this.data.filterResult.fromDate],
    toDate: [this.data.filterResult.toDate],
    questionnaireIds: [this.data.filterResult.questionnaireIds],
    examTypeIds: [this.data.filterResult.examTypeIds],
    classIds: [this.data.filterResult.classIds],
    gradeIds: [this.data.filterResult.gradeIds]
  });

  onClose() {
    this.dialogRef.close(null);
  }

  onSubmit() {
    const { fromDate, toDate, questionnaireIds, examTypeIds, classIds, gradeIds } = this.form.value;
    const filterResult: FilterResult = {
      fromDate, toDate, questionnaireIds, examTypeIds, classIds, gradeIds
    };
    this.dialogRef.close(filterResult);
  }
}
