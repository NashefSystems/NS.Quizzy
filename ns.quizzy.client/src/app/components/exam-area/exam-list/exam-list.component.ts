import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DialogService } from '../../../services/dialog.service';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ConfirmDialogComponent } from '../../global-area/confirm-dialog/confirm-dialog.component';
import { TableColumnInfo } from '../../global-area/table/table-column-info';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { ExamsService } from '../../../services/backend/exams.service';
import { IExamDto } from '../../../models/backend/exam.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { forkJoin } from 'rxjs';
import moment from 'moment';

@Component({
  selector: 'app-exam-list',
  standalone: false,
  templateUrl: './exam-list.component.html',
  styleUrl: './exam-list.component.scss'
})
export class ExamListComponent implements OnInit {
  private readonly _examsService = inject(ExamsService);
  private readonly _examTypesService = inject(ExamTypesService);
  private readonly _questionnairesService = inject(QuestionnairesService);
  private readonly _router = inject(Router);
  private readonly _dialogService = inject(DialogService);

  questionnaires: IQuestionnaireDto[];
  examTypes: IExamTypeDto[];
  items: IExamDto[];
  columns: TableColumnInfo[] = [
    {
      key: 'startTime',
      title: 'EXAM_AREA.START_TIME',
      converter: (item: IExamDto) => moment(item.startTime).format('DD/MM/YYYY HH:mm')
    },
    {
      key: 'questionnaireId',
      title: 'EXAM_AREA.QUESTIONNAIRE',
      converter: (item: IExamDto) => {
        const element = this.questionnaires?.find(x => x.id === item.questionnaireId);
        if (!element) {
          return '';
        }
        return `(${element.code}) ${element.name}`;
      }
    },
    {
      key: 'examTypeId',
      title: 'EXAM_AREA.EXAM_TYPE',
      converter: (item: IExamDto) => {
        const element = this.examTypes?.find(x => x.id === item.examTypeId);
        return element?.name;
      }
    },
    {
      key: 'duration',
      title: 'EXAM_AREA.DURATION',
      converter: (item: IExamDto) => `${this.getTimeFormat(item.duration)} (${this.getTimeFormat(item.durationWithExtra)})`
    },
  ];

  getTimeFormat(value: string) {
    const [h, m, s] = value.split(':');
    return `${h}:${m}`;
  }

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    forkJoin([
      this._questionnairesService.get(),
      this._examTypesService.get(),
    ]).subscribe(([questionnaires, examTypes]) => {
      this.questionnaires = questionnaires;
      this.examTypes = examTypes;

      this._examsService.get().subscribe({
        next: (responseBody) => this.items = responseBody
      });
    });
  }

  onAdd() {
    this._router.navigate(['/exams/new']);
  };

  onEdit(item: IExamDto) {
    this._router.navigate([`/exams/edit/${item?.id}`]);
  };

  onDelete(item: IExamDto) {
    const questionnaire = this.questionnaires?.find(x => x.id === item.questionnaireId);
    const examType = this.examTypes?.find(x => x.id === item.examTypeId);
    const startTime = moment(item.startTime).format('DD/MM/YYYY HH:mm');
    const itemName = `(${questionnaire?.code}) ${questionnaire?.name} | ${examType?.name} | ${startTime}`;
    const dialogPayload: OpenDialogPayload = {
      component: ConfirmDialogComponent,
      isFullDialog: false,
      data: {
        itemName: itemName
      }
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then(isConfirmed => {
        if (isConfirmed) {
          this._examsService.delete(item.id).subscribe({
            next: () => this.loadData()
          });
        }
      });
  };
}
