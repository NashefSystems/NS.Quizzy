import { Component, EventEmitter, inject, Input, viewChild } from '@angular/core';
import { IExamDto } from '../../../models/backend/exam.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { MatAccordion } from '@angular/material/expansion';
import { AppTranslateService } from '../../../services/app-translate.service';

@Component({
  selector: 'app-exam-schedule-list',
  standalone: false,
  templateUrl: './exam-schedule-list.component.html',
  styleUrl: './exam-schedule-list.component.scss'
})
export class ExamScheduleListComponent {
  isLoading: boolean = false;
  @Input() exams: IExamDto[] = [];
  @Input() grades: IGradeDto[] = [];
  @Input() classes: IClassDto[] = [];
  @Input() questionnaires: IQuestionnaireDto[] = [];
  @Input() examTypes: IExamTypeDto[] = [];
  @Input() subjects: ISubjectDto[] = [];
  @Input() filterIsActive: boolean;
  @Input() onFilter = new EventEmitter<void>();

  gradesDic: { [key: string]: IGradeDto } = {};
  classesDic: { [key: string]: IClassDto } = {};
  questionnairesDic: { [key: string]: IQuestionnaireDto } = {};
  examTypesDic: { [key: string]: IExamTypeDto } = {};
  subjectsDic: { [key: string]: ISubjectDto } = {};
  accordion = viewChild.required(MatAccordion);

  private readonly _appTranslateService = inject(AppTranslateService);

  ngOnChanges(): void {
    this.isLoading = false;
    this.gradesDic = Object.fromEntries(this.grades.map(x => [x.id, x]));
    this.classesDic = Object.fromEntries(this.classes.map(x => [x.id, x]));
    this.questionnairesDic = Object.fromEntries(this.questionnaires.map(x => [x.id, x]));
    this.examTypesDic = Object.fromEntries(this.examTypes.map(x => [x.id, x]));
    this.subjectsDic = Object.fromEntries(this.subjects.map(x => [x.id, x]));

    setTimeout(() => {
      this.isLoading = true;
    }, 100);
  }

  getDescription(exam: IExamDto) {
    const questionnaire = this.questionnaires.find(x => x.id === exam.questionnaireId);
    const examType = this.examTypes.find(x => x.id === exam.examTypeId);
    return `(${questionnaire?.code}) ${questionnaire?.name} - ${examType?.name}`;
  }

  onPrint() {
    this.accordion().openAll();
    setTimeout(() => {
      this.printDiv('exam-list');
    }, 100);
  }

  printDiv(elementId: string) {
    const printContent = document.getElementById(elementId)?.outerHTML;
    if (!printContent) {
      console.error('Print content not found');
      return;
    }

    // Extract all stylesheets safely
    let styles = '';
    for (const styleSheet of Array.from(document.styleSheets)) {
      try {
        styles += Array.from(styleSheet.cssRules).map(rule => rule.cssText).join('\n');
      } catch (e) {
        console.warn('Skipping cross-origin stylesheet:', e);
      }
    }

    // Open new print window
    const printWindow = window.open('', '_blank');
    if (!printWindow) {
      console.error('Failed to open print window');
      return;
    }

    const dir = this._appTranslateService.translate("DIR");
    const title = this._appTranslateService.translate("PAGE_TITLES.EXAM_SCHEDULE");

    printWindow.document.open(); // Ensure the document is writable
    printWindow.document.write(`
      <html>
        <head>
          <title>${title}</title>
          <style>${styles}</style>
          <style>
            body {
              padding: 16px;
              background-color: white;
            }
            .page-title {
              margin-bottom: 16px;
              text-align: center;
            }
          </style>
        </head>
        <body dir="${dir}">
          <h1 class="page-title">${title}</h1>  
          ${printContent}
        </body>
      </html>
    `);
    printWindow.document.close(); // Close document to trigger rendering

    // Wait for the window to load before printing
    printWindow.onload = () => {
      setTimeout(() => {
        printWindow.print();
        printWindow.close();
      }, 100);
    };
  }

  onFilterClick() {
    this.onFilter.emit();
  }

  getClasses(ids: string[]) {
    if (!ids?.length || !this.classes?.length) {
      return '';
    }
    var names = this.classes.filter(x => ids.includes(x.id)).map(x => x.name);
    return names.join(' | ');
  }

  getGrades(ids?: string[]) {
    if (!ids?.length || !this.grades?.length) {
      return '';
    }
    var names = this.grades.filter(x => ids.includes(x.id)).map(x => x.name);
    return names.join(' | ');
  }
}
