import { Component, Input } from '@angular/core';
import { IExamDto } from '../../../models/backend/exam.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ISubjectDto } from '../../../models/backend/subject.dto';

@Component({
  selector: 'app-exam-schedule-calendar',
  standalone: false,

  templateUrl: './exam-schedule-calendar.component.html',
  styleUrl: './exam-schedule-calendar.component.scss'
})
export class ExamScheduleCalendarComponent {
  @Input() exams: IExamDto[] = [];
  @Input() grades: IGradeDto[] = [];
  @Input() classes: IClassDto[] = [];
  @Input() questionnaires: IQuestionnaireDto[] = [];
  @Input() examTypes: IExamTypeDto[] = [];
  @Input() subjects: ISubjectDto[] = [];
}
