import { Component, inject, OnInit } from '@angular/core';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { SubjectsService } from '../../../services/backend/subjects.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { ConfirmDialogComponent } from '../../confirm-dialog/confirm-dialog.component';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';

@Component({
  selector: 'app-subject-list',
  standalone: false,

  templateUrl: './subject-list.component.html',
  styleUrl: './subject-list.component.scss'
})
export class SubjectListComponent implements OnInit {
  private readonly _subjectsService = inject(SubjectsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);


  items: ISubjectDto[];
  columns: { [key: string]: string } = {
    'name': 'SUBJECT_AREA.NAME',
    'itemOrder': 'SUBJECT_AREA.ITEM_ORDER'
  };

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._subjectsService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    this._router.navigate(['/subjects/new']);
  };

  onEdit(item: ISubjectDto) {
    this._router.navigate([`/subjects/edit/${item?.id}`]);
  };

  onDelete(item: ISubjectDto) {
    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: item.name
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._subjectsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
