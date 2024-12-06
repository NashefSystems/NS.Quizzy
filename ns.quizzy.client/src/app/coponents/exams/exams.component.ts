import { Component, OnInit } from '@angular/core';
import { ExamListFilterComponent } from '../exam-list-filter/exam-list-filter.component';
import { DialogService } from '../../services/dialog.service';
import { AccountService } from '../../services/account.service';
import { TestDialogComponent } from '../dialogs/test-dialog/test-dialog.component';

@Component({
  selector: 'app-exams',
  templateUrl: './exams.component.html',
  styleUrl: './exams.component.scss'
})
export class ExamsComponent implements OnInit {
  apiValue = "init";

  constructor(
    private readonly dialogService: DialogService,
    private readonly accountService: AccountService,
  ) { }

  ngOnInit(): void {
    this.accountService.getTest().subscribe({
      next: (result) => {
        this.apiValue = result;
      },
      error: (error) => {
        console.error(error);
      }
    });
  }

  openFilter() {
    const dialogRef = this.dialogService.openDialog(TestDialogComponent, null)
      .then(result => {
        console.log('The dialog was closed', result);
        if (result !== undefined) {
          // this.animal.set(result);
        }
      });
  }
}
