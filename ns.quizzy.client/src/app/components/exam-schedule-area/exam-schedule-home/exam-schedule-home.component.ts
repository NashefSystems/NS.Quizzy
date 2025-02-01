import { Component, inject } from '@angular/core';
import { GradesService } from '../../../services/backend/grades.service';
import { ClassesService } from '../../../services/backend/classes.service';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { ExamsService } from '../../../services/backend/exams.service';
import { IClassDto } from '../../../models/backend/class.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { forkJoin } from 'rxjs';
import { IExamDto, IExamFilterRequest } from '../../../models/backend/exam.dto';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { DialogAction, DialogResult, ExamScheduleFilterData, FilterResult } from '../exam-schedule-filter/exam-schedule-filter.data';
import { ExamScheduleFilterComponent } from '../exam-schedule-filter/exam-schedule-filter.component';
import { DialogService } from '../../../services/dialog.service';
import { SubjectsService } from '../../../services/backend/subjects.service';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { NotificationsService } from '../../../services/notifications.service';
import { DateTimeUtils } from '../../../utils/date-time.utils';
import { StorageService } from '../../../services/storage.service';
import { LocalStorageKeys } from '../../../enums/local-storage-keys.enum';

@Component({
  selector: 'app-exam-schedule-home',
  standalone: false,
  templateUrl: './exam-schedule-home.component.html',
  styleUrl: './exam-schedule-home.component.scss'
})
export class ExamScheduleHomeComponent {
  private readonly _dialogService = inject(DialogService);
  private readonly _gradesService = inject(GradesService);
  private readonly _classesService = inject(ClassesService);
  private readonly _questionnairesService = inject(QuestionnairesService);
  private readonly _examTypesService = inject(ExamTypesService);
  private readonly _subjectsService = inject(SubjectsService);
  private readonly _examsService = inject(ExamsService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _storageService = inject(StorageService);

  exams: IExamDto[] = [];
  grades: IGradeDto[] = [];
  classes: IClassDto[] = [];
  questionnaires: IQuestionnaireDto[] = [];
  examTypes: IExamTypeDto[] = [];
  subjects: ISubjectDto[] = [];
  filterData: FilterResult;
  filterIsActive: boolean = false;

  ngOnInit(): void {
    this.setExamFilterData();
    this.loadData();
  }

  private setExamFilterData() {
    const examFilterDataJson = this._storageService.getLocalStorage(LocalStorageKeys.examFilterData);

    if (examFilterDataJson) {
      console.log("load ExamFilterData from cache");
      this.filterData = JSON.parse(examFilterDataJson);
      this.filterIsActive = true;
    }

    if (!this.filterData) {
      this.setDefaultFilterData();
    }
  }

  private setDefaultFilterData() {
    const now = new Date();
    const toDate = new Date(now);
    toDate.setMonth(now.getMonth() + 3);

    this.filterIsActive = false;
    this.filterData = {
      fromDate: DateTimeUtils.getDateTimeFromIso(now.toISOString(), 'YYYY-MM-DD'),
      toDate: DateTimeUtils.getDateTimeFromIso(toDate.toISOString(), 'YYYY-MM-DD'),
      classIds: [],
      examTypeIds: [],
      gradeIds: [],
      questionnaireIds: [],
      subjectIds: [],
    };
  }

  loadData() {
    forkJoin([
      this._gradesService.get(),
      this._classesService.get(),
      this._questionnairesService.get(),
      this._examTypesService.get(),
      this._subjectsService.get(),
    ]).subscribe(([grades, classes, questionnaires, examTypes, subjects]) => {
      this.grades = grades;
      this.classes = classes;
      this.questionnaires = questionnaires;
      this.examTypes = examTypes;
      this.subjects = subjects;
      this.getExams();
    });
  }

  onFilter() {
    const dialogData: ExamScheduleFilterData = {
      classes: this.classes,
      examTypes: this.examTypes,
      grades: this.grades,
      questionnaires: this.questionnaires,
      subjects: this.subjects,
      filterResult: this.filterData,
    }
    const dialogPayload: OpenDialogPayload = {
      component: ExamScheduleFilterComponent,
      isFullDialog: false,
      data: dialogData
    };
    this._dialogService
      .openDialog(dialogPayload)
      .then((filterResult) => {
        if (filterResult) {
          const dialogResult = filterResult as DialogResult;
          switch (dialogResult.action) {
            case DialogAction.CLOSE:
              return;
            case DialogAction.CLEAR:
              this.setDefaultFilterData();
              this._storageService.removeLocalStorage(LocalStorageKeys.examFilterData);

              break;
            case DialogAction.SUBMIT:
              if (dialogResult.filterResult) {
                this.filterData = dialogResult.filterResult;
                this._storageService.setLocalStorage(LocalStorageKeys.examFilterData, JSON.stringify(this.filterData));
                this.filterIsActive = true;
              } else {
                this.setDefaultFilterData();
              }
              break;
          }
          this.getExams();
        }
      });
  }

  getExams() {
    const request: IExamFilterRequest = {
      fromTime: DateTimeUtils.getDateTimeAsIso(this.filterData.fromDate),
      toTime: DateTimeUtils.getDateTimeAsIso(this.filterData.toDate + "T23:59:59"),
      classIds: this.filterData.classIds,
      examTypeIds: this.filterData.examTypeIds,
      gradeIds: this.filterData.gradeIds,
      questionnaireIds: this.filterData.questionnaireIds,
      subjectIds: this.filterData.subjectIds,
    };
    this._examsService.filter(request).subscribe({
      next: result => {
        this.exams = result;
      },
      error: err => {
        this._notificationsService.httpErrorHandler(err);
      }
    });
  }
}
