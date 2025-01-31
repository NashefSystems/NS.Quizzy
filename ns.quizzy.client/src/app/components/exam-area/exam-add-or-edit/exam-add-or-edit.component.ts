import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto, IClassPayloadDto } from '../../../models/backend/class.dto';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { forkJoin } from 'rxjs';
import moment from 'moment';
import { IExamPayloadDto } from '../../../models/backend/exam.dto';
import { ExamsService } from '../../../services/backend/exams.service';

@Component({
  selector: 'app-exam-add-or-edit',
  standalone: false,

  templateUrl: './exam-add-or-edit.component.html',
  styleUrl: './exam-add-or-edit.component.scss'
})
export class ExamAddOrEditComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);

  private readonly _gradesService = inject(GradesService);
  private readonly _classesService = inject(ClassesService);
  private readonly _questionnairesService = inject(QuestionnairesService);
  private readonly _examTypesService = inject(ExamTypesService);
  private readonly _examsService = inject(ExamsService);

  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;

  classes: IClassDto[] = [];
  classesFiltered: IClassDto[] = [];
  grades: IGradeDto[] = [];
  questionnaires: IQuestionnaireDto[] = [];
  examTypes: IExamTypeDto[] = [];

  form: FormGroup = this._fb.group({
    startTime: ['', [Validators.required]],
    questionnaireId: ['', [Validators.required]],
    examTypeId: ['', [Validators.required]],
    duration: ['', [Validators.required]],
    durationWithExtra: ['', [Validators.required]],
    classIds: [''],
    gradeIds: ['']
  });

  ngOnInit(): void {
    this._activatedRoute.paramMap.subscribe(params => {
      this.id = params.get('id');
      this.loadDataFromId(this.id);
    });
  }

  getDateTimeAsIso(rawValue: string) {
    console.log("getDateTimeAsIso", rawValue);
    if (!rawValue) {
      return '';
    }
    return moment(rawValue).format('YYYY-MM-DDTHH:mm:ss.SSSZ')
  }


  getDateTimeFromIso(rawValue: string) {
    console.log("getDateTimeFromIso", rawValue);
    if (!rawValue) {
      return '';
    }
    return moment(rawValue).format('YYYY-MM-DDTHH:mm')
  }

  loadDataFromId(id: string | null) {
    forkJoin([
      this._gradesService.get(),
      this._classesService.get(),
      this._questionnairesService.get(),
      this._examTypesService.get(),
    ]).subscribe(([grades, classes, questionnaires, examTypes]) => {
      this.grades = grades;
      this.classes = this.classesFiltered = classes;
      this.questionnaires = questionnaires;
      this.examTypes = examTypes;

      if (id) {
        this._examsService.getById(id).subscribe({
          next: data => {
            const { startTime, questionnaireId, examTypeId, duration, durationWithExtra, classIds, gradeIds } = data;
            const newValue = {
              ...this.form.value,
              startTime: this.getDateTimeFromIso(startTime), questionnaireId, examTypeId, duration, durationWithExtra, classIds, gradeIds
            };
            this.form.setValue(newValue);
            this.setClassesFiltered();
          }
        });
      }
    });
  }

  setClassesFiltered() {
    const gradeIds: string[] = this.form?.value?.gradeIds ?? [];
    this.classesFiltered = [...this.classes.filter(x => !gradeIds.includes(x.gradeId))];

    let classIds: string[] = [...(this.form?.value?.classIds ?? [])];
    if (classIds?.length > 0) {
      const classesFilteredIds = this.classesFiltered.map(x => x.id);
      let newClassIds = classIds?.filter(x => classesFilteredIds.includes(x));
      this.form.setValue({
        ...this.form.value,
        classIds: newClassIds
      });
    }
  }

  onGradeIdsChange() {
    this.setClassesFiltered();
  }

  onQuestionnaireIdChange() {
    const questionnaire = this.questionnaires.find(x => x.id === this.form?.value?.questionnaireId);
    if (!questionnaire) {
      return;
    }

    this.form.setValue({
      ...this.form.value,
      duration: questionnaire.duration,
      durationWithExtra: questionnaire.durationWithExtra,
    });
  }

  onSubmit() {
    if (!this.form.valid) {
      return;
    }
    const { startTime, questionnaireId, examTypeId, duration, durationWithExtra, classIds, gradeIds } = this.form.value;

    const payload: IExamPayloadDto = {
      startTime: this.getDateTimeAsIso(startTime),
      questionnaireId: questionnaireId,
      examTypeId: examTypeId,
      duration: duration,
      durationWithExtra: durationWithExtra,
      gradeIds: gradeIds,
      classIds: classIds,
    };

    if (this.id) {
      this._examsService.update(this.id, payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_UPDATED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    } else {
      this._examsService.insert(payload).subscribe({
        next: responseBody => {
          this._notificationsService.success('ITEM_ADDED_SUCCESSFULLY');
          this.navigateToList();
        },
        error: err => {
          this._notificationsService.httpErrorHandler(err);
        }
      });
    }
  }

  navigateToList() {
    this._router.navigate(['/exams']);
  }
}
