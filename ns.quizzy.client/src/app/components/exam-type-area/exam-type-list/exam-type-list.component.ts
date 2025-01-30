import { Component, inject, OnInit } from '@angular/core';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { TableColumnInfo } from '../../global-area/table/table-column-info';

@Component({
  selector: 'app-exam-type-list',
  standalone: false,
  templateUrl: './exam-type-list.component.html',
  styleUrl: './exam-type-list.component.scss'
})
export class ExamTypeListComponent implements OnInit {
  private readonly _examTypesService = inject(ExamTypesService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);


  items: IExamTypeDto[];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'EXAM_TYPE_AREA.NAME'
    }, {
      key: 'itemOrder',
      title: 'EXAM_TYPE_AREA.ITEM_ORDER'
    }
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._examTypesService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    this._router.navigate(['/exam-types/new']);
  };

  onEdit(item: IExamTypeDto) {
    this._router.navigate([`/exam-types/edit/${item?.id}`]);
  };

  onDelete(item: IExamTypeDto) {
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
          this._examTypesService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
