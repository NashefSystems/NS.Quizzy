import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradeDto } from '../../../models/backend/grade.dto';

@Component({
  selector: 'app-grade-list',
  standalone: false,
  templateUrl: './grade-list.component.html',
  styleUrl: './grade-list.component.scss'
})
export class GradeListComponent implements OnInit {
  private readonly _gradesService = inject(GradesService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);


  items: IGradeDto[];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'GRADE_AREA.NAME'
    },
    {
      key: 'code',
      title: 'GRADE_AREA.CODE'
    },
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._gradesService.get().subscribe({
      next: (responseBody) => this.items = responseBody
    });
  }

  onAdd() {
    this._router.navigate(['/grades/new']);
  };

  onEdit(item: IGradeDto) {
    this._router.navigate([`/grades/edit/${item?.id}`]);
  };

  onDelete(item: IGradeDto) {
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
          this._gradesService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
