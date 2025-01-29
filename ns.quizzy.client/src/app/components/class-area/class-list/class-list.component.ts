import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../confirm-dialog/confirm-dialog.component';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';
import { TableColumnInfo } from '../../table/table-column-info';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradeDto } from '../../../models/backend/grade.dto';

@Component({
  selector: 'app-class-list',
  standalone: false,

  templateUrl: './class-list.component.html',
  styleUrl: './class-list.component.scss'
})
export class ClassListComponent implements OnInit {
  private readonly _classesService = inject(ClassesService);
  private readonly _gradesService = inject(GradesService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  grades: IGradeDto[];
  items: IClassDto[];
  columns: TableColumnInfo[] = [
    {
      key: 'name',
      title: 'CLASS_AREA.NAME'
    },
    {
      key: 'gradeId',
      title: 'CLASS_AREA.GRADE_ID',
      converter: (item: IClassDto) => {
        const parent = this.grades.find(x => x.id === item.gradeId);
        return parent?.name;
      }
    },
    {
      key: 'fullCode',
      title: 'CLASS_AREA.FULL_CODE'
    },
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._gradesService.get().subscribe({
      next: (responseBody) => {
        this.grades = responseBody;
        this._classesService.get().subscribe({
          next: (responseBody) => this.items = responseBody
        });
      }
    });
  }

  onAdd() {
    this._router.navigate(['/classes/new']);
  };

  onEdit(item: IClassDto) {
    this._router.navigate([`/classes/edit/${item?.id}`]);
  };

  onDelete(item: IClassDto) {
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
          this._classesService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
