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
import { FormBuilder, FormGroup } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { IExamDto } from '../../../models/backend/exam.dto';
import { OpenDialogPayload } from '../../../models/dialog/open-dialog.payload';
import { ExamScheduleFilterData, FilterResult } from '../exam-schedule-filter/exam-schedule-filter.data';
import { ExamScheduleFilterComponent } from '../exam-schedule-filter/exam-schedule-filter.component';
import { DialogService } from '../../../services/dialog.service';

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
  private readonly _examsService = inject(ExamsService);
  private readonly _fb = inject(FormBuilder);

  exams: IExamDto[] = [];
  grades: IGradeDto[] = [];
  classes: IClassDto[] = [];
  questionnaires: IQuestionnaireDto[] = [];
  examTypes: IExamTypeDto[] = [];
  
  filterData: FilterResult = {
    fromDate: '',
    toDate: '',
    classIds: [],
    examTypeIds: [],
    gradeIds: [],
    questionnaireIds: [],
  };

  form: FormGroup = this._fb.group({
    fromTime: [''],
    toTime: [''],
    questionnaireIds: [''],
    examTypeIds: [''],
    classIds: [''],
    gradeIds: ['']
  });

  ngOnInit(): void {
    this.loadData();
  }

  loadData() {
    forkJoin([
      this._gradesService.get(),
      this._classesService.get(),
      this._questionnairesService.get(),
      this._examTypesService.get(),
    ]).subscribe(([grades, classes, questionnaires, examTypes]) => {
      this.grades = grades;
      this.classes = classes;
      this.questionnaires = questionnaires;
      this.examTypes = examTypes;
    });
  }

  onFilter() {
    const dialogData: ExamScheduleFilterData = {
      classes: this.classes,
      examTypes: this.examTypes,
      grades: this.grades,
      questionnaires: this.questionnaires,
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
          this.filterData = filterResult as FilterResult;
          console.log("filterData: ", this.filterData);
          this.refresh();
        }
      });
  }

  refresh() {

  }

}
