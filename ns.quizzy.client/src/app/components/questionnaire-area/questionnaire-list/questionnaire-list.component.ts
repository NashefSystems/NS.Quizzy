import { Component, inject, OnInit } from '@angular/core';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { SubjectsService } from '../../../services/backend/subjects.service';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-questionnaire-list',
  standalone: false,
  templateUrl: './questionnaire-list.component.html',
  styleUrl: './questionnaire-list.component.scss'
})
export class QuestionnaireListComponent implements OnInit {
  private readonly _questionnairesService = inject(QuestionnairesService);
  private readonly _subjectsService = inject(SubjectsService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  subjects: ISubjectDto[] = [];
  items: IQuestionnaireDto[] = [];
  columns: TableColumnInfo[] = [
    {
      key: 'code',
      title: 'QUESTIONNAIRE_AREA.CODE'
    },
    {
      key: 'name',
      title: 'QUESTIONNAIRE_AREA.NAME'
    },
    {
      key: 'subjectId',
      title: 'QUESTIONNAIRE_AREA.SUBJECT_ID',
      converter: (item: IQuestionnaireDto) => {
        const parent = this.subjects.find(x => x.id === item.subjectId);
        return parent?.name;
      }
    },
  ];

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    this._subjectsService.get().subscribe({
      next: (responseBody) => {
        this.subjects = responseBody;
        this._questionnairesService.get().subscribe({
          next: (responseBody) => this.items = responseBody
        });
      }
    });
  }

  onAdd() {
    this._router.navigate(['/questionnaires/new']);
  };

  onEdit(item: IQuestionnaireDto) {
    this._router.navigate([`/questionnaires/edit/${item?.id}`]);
  };

  onDelete(item: IQuestionnaireDto) {
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
          this._questionnairesService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
