import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { NotificationsService } from '../../../services/notifications.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassesService } from '../../../services/backend/classes.service';
import { IClassDto } from '../../../models/backend/class.dto';
import { GradesService } from '../../../services/backend/grades.service';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { QuestionnairesService } from '../../../services/backend/questionnaires.service';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ExamTypesService } from '../../../services/backend/exam-types.service';
import { forkJoin } from 'rxjs';
import { IExamPayloadDto } from '../../../models/backend/exam.dto';
import { ExamsService } from '../../../services/backend/exams.service';
import { DateTimeUtils } from '../../../utils/date-time.utils';
import { MoedsService } from '../../../services/backend/moeds.service';
import { IMoedDto } from '../../../models/backend/moed.dto';

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
  private readonly _moedsService = inject(MoedsService);
  private readonly _examsService = inject(ExamsService);

  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _activatedRoute = inject(ActivatedRoute);
  id: string | null = null;

  classes: IClassDto[] = [];
  classesFiltered: IClassDto[] = [];
  improvementClasses: { [key: string]: boolean } = {};

  grades: IGradeDto[] = [];
  improvementGrades: { [key: string]: boolean } = {};

  questionnaires: IQuestionnaireDto[] = [];
  examTypes: IExamTypeDto[] = [];
  moeds: IMoedDto[] = [];

  form: FormGroup = this._fb.group({
    startTime: ['', [Validators.required]],
    questionnaireId: ['', [Validators.required]],
    examTypeId: ['', [Validators.required]],
    moedId: ['', [Validators.required]],
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

  loadDataFromId(id: string | null) {
    forkJoin([
      this._gradesService.get(),
      this._classesService.get(),
      this._questionnairesService.get(),
      this._examTypesService.get(),
      this._moedsService.get(),
    ]).subscribe(([grades, classes, questionnaires, examTypes, moeds]) => {
      this.grades = grades;
      this.improvementGrades = Object.fromEntries(this.grades.map(x => [x.id, false]));
      this.classes = this.classesFiltered = classes;
      this.improvementClasses = Object.fromEntries(this.classes.map(x => [x.id, false]));
      this.questionnaires = questionnaires;
      this.examTypes = examTypes;
      this.moeds = moeds;

      if (id) {
        this._examsService.getById(id).subscribe({
          next: data => {
            const { startTime, questionnaireId, examTypeId, duration, durationWithExtra, moedId } = data;
            const classIds = [...data.classIds || [], ...data.improvementClassIds || []];
            const gradeIds = [...data.gradeIds || [], ...data.improvementGradeIds || []];
            const newValue = {
              ...this.form.value,
              startTime: DateTimeUtils.getDateTimeFromIso(startTime), questionnaireId, examTypeId, duration, durationWithExtra, classIds, gradeIds, moedId
            };
            data.improvementGradeIds?.forEach(x => this.improvementGrades[x] = true);
            data.improvementClassIds?.forEach(x => this.improvementClasses[x] = true);
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

  isSelectedGrade(id: string) {
    const { gradeIds } = this.form.value;
    return gradeIds?.includes(id) ?? false;
  }

  getSelectedGradeNames() {
    const { gradeIds } = this.form.value;
    return this.grades
      .filter(g => gradeIds.includes(g.id))
      .map(g => g.name)
      .join(', ');
  }

  isSelectedClass(id: string) {
    const { classIds } = this.form.value;
    return classIds?.includes(id) ?? false;
  }

  getSelectedClassNames() {
    const { classIds } = this.form.value;
    return this.classes
      .filter(g => classIds.includes(g.id))
      .map(g => g.name)
      .join(', ');
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
    const { startTime, questionnaireId, examTypeId, duration, durationWithExtra, classIds, gradeIds, moedId } = this.form.value;

    const selectedGradeIds = (gradeIds as string[]) || [];
    const selectedClassIds = (classIds as string[]) || [];

    const payload: IExamPayloadDto = {
      startTime: DateTimeUtils.getDateTimeAsIso(startTime),
      questionnaireId: questionnaireId,
      examTypeId: examTypeId,
      moedId: moedId,
      duration: duration,
      durationWithExtra: durationWithExtra,
      gradeIds: selectedGradeIds?.filter(x => !this.improvementGrades[x]),
      improvementGradeIds: selectedGradeIds?.filter(x => this.improvementGrades[x]),
      classIds: selectedClassIds?.filter(x => !this.improvementClasses[x]),
      improvementClassIds: selectedClassIds?.filter(x => this.improvementClasses[x]),
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
