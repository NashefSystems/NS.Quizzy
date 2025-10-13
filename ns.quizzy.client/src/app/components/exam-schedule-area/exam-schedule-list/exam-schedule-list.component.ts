import { Component, EventEmitter, inject, Input, OnInit, viewChild } from '@angular/core';
import { IExamDto } from '../../../models/backend/exam.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { MatAccordion } from '@angular/material/expansion';
import { IMoedDto } from '../../../models/backend/moed.dto';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';
import { IExportDataItem, ExportService } from './export.service';
import { DateTimeUtils } from '../../../utils/date-time.utils';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';
import { TimePipe } from '../../../pipes/time.pipe';
import { AccountService } from '../../../services/backend/account.service';
import { CheckPermissionsUtils } from '../../../utils/check-permissions.utils';
import { ExamsService } from '../../../services/backend/exams.service';
import { GlobalService } from '../../../services/global.service';
import { FeatureFlags } from '../../../enums/feature-flags.enum';

@Component({
  selector: 'app-exam-schedule-list',
  standalone: false,
  templateUrl: './exam-schedule-list.component.html',
  styleUrl: './exam-schedule-list.component.scss'
})
export class ExamScheduleListComponent implements OnInit {
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  private readonly _exportService = inject(ExportService);
  private readonly _timePipe = inject(TimePipe);
  private readonly _accountService = inject(AccountService);
  private readonly _examsService = inject(ExamsService);
  private readonly _globalService = inject(GlobalService);

  @Input()
  set exams(value: IExamDto[]) {
    this._exams = value;
    this.onExamsChange(value);
  }
  get exams(): IExamDto[] {
    return this._exams;
  }

  @Input() grades: IGradeDto[] = [];
  @Input() classes: IClassDto[] = [];
  @Input() questionnaires: IQuestionnaireDto[] = [];
  @Input() examTypes: IExamTypeDto[] = [];
  @Input() moeds: IMoedDto[] = [];
  @Input() subjects: ISubjectDto[] = [];
  @Input() filterIsActive: boolean;
  @Input() onFilter = new EventEmitter<void>();

  downloadFileIsAvailable: boolean = false;
  isEditor: boolean = false;
  isLoading: boolean = true;
  iconColor: string = '#0053E7';
  appVersion: string = "";
  nativeAppIsAvailable: boolean | null = null;
  searchValue: string = '';
  _exams: IExamDto[] = [];
  filteredExams: IExamDto[] = [];
  gradesDic: { [key: string]: IGradeDto } = {};
  classesDic: { [key: string]: IClassDto } = {};
  questionnairesDic: { [key: string]: IQuestionnaireDto } = {};
  examTypesDic: { [key: string]: IExamTypeDto } = {};
  moedsDic: { [key: string]: IMoedDto } = {};
  subjectsDic: { [key: string]: ISubjectDto } = {};
  accordion = viewChild.required(MatAccordion);

  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);
  ExportService: any;

  ngOnInit(): void {
    this.nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
    this._clientAppSettingsService.get().subscribe({ next: result => this.appVersion = result?.AppVersion });
    this.filteredExams = [...this.exams];
    this._accountService.getDetails().subscribe({
      next: data => this.isEditor = CheckPermissionsUtils.isAdminUser(data)
    });

    this._globalService
      .featureIsAvailableAsync(FeatureFlags.EXAM_SCHEDULE_LIST__DOWNLOAD_FILE)
      .then(x => this.downloadFileIsAvailable = x);
  }

  ngOnChanges(): void {
    this.isLoading = true;

    this.gradesDic = Object.fromEntries(this.grades.map(x => [x.id, x]));
    this.classesDic = Object.fromEntries(this.classes.map(x => [x.id, x]));
    this.questionnairesDic = Object.fromEntries(this.questionnaires.map(x => [x.id, x]));
    this.examTypesDic = Object.fromEntries(this.examTypes.map(x => [x.id, x]));
    this.moedsDic = Object.fromEntries(this.moeds.map(x => [x.id, x]));
    this.subjectsDic = Object.fromEntries(this.subjects.map(x => [x.id, x]));

    setTimeout(() => {
      this.isLoading = false;
    }, 100);
  }

  private onExamsChange(exams: IExamDto[]): void {
    this._exams = [...exams];
    this.applyFilter();
  }

  // onPrint() {
  //   this.accordion().openAll();
  //   setTimeout(() => {
  //     this.printDiv('exam-list');
  //   }, 100);
  // }

  // printDiv(elementId: string) {
  //   const printContent = document.getElementById(elementId)?.outerHTML;
  //   if (!printContent) {
  //     console.error('Print content not found');
  //     return;
  //   }

  //   // Extract all stylesheets safely
  //   let styles = '';
  //   for (const styleSheet of Array.from(document.styleSheets)) {
  //     try {
  //       styles += Array.from(styleSheet.cssRules).map(rule => rule.cssText).join('\n');
  //     } catch (e) {
  //       console.warn('Skipping cross-origin stylesheet:', e);
  //     }
  //   }

  //   const dir = this._appTranslateService.translate("DIR");
  //   const title = this._appTranslateService.translate("PAGE_TITLES.EXAM_SCHEDULE");

  //   // Create an iframe for printing
  //   const printFrame = document.createElement('iframe');
  //   printFrame.style.position = 'absolute';
  //   printFrame.style.width = '0px';
  //   printFrame.style.height = '0px';
  //   printFrame.style.border = 'none';
  //   document.body.appendChild(printFrame);

  //   const printDoc = printFrame.contentWindow?.document;
  //   if (!printDoc) {
  //     console.error('Failed to create print document');
  //     return;
  //   }

  //   printDoc.open();
  //   printDoc.write(`
  //     <html>
  //       <head>
  //         <title>${title}</title>
  //         <style>${styles}</style>
  //         <style>
  //           body {
  //             padding: 16px;
  //             background-color: white;
  //           }
  //           .page-title {
  //             margin-bottom: 16px;
  //             text-align: center;
  //           }
  //           .app-info {
  //               margin-top: 1rem;
  //               display: flex;
  //               justify-content: center;
  //               color: gray;
  //               direction: ltr;
  //           }
  //         </style>
  //       </head>
  //       <body dir="${dir}">
  //         <h1 class="page-title">${title}</h1>            
  //         ${printContent}
  //         <div class="app-info">
  //             <small>Quizzy App (V${this.appVersion})</small>
  //         </div>
  //       </body>
  //     </html>
  //   `);
  //   printDoc.close();

  //   printFrame.contentWindow?.focus();
  //   printFrame.contentWindow?.print();

  //   // Remove the iframe after printing (wait a bit to ensure printing starts)
  //   setTimeout(() => {
  //     document.body.removeChild(printFrame);
  //   }, 500);
  // }

  isFilteredExam(exam: IExamDto, filterValue: string): boolean {
    filterValue = filterValue.trim();
    let strValue = `
        ${this.getQuestionnaireInfo(exam)}
        ${exam.startTimeStr}
        ${this.getDay(exam.startTime)}
        ${this.examTypesDic[exam.examTypeId].name}
        ${this.moedsDic[exam.moedId].name}
        ${this.getSubjectName(exam)}
        ${this._timePipe.transform(exam.duration, 'HH:mm')}
        ${this._timePipe.transform(exam.durationWithExtra, 'HH:mm')}
        ${(exam.gradeIds && exam.gradeIds.length > 0) ? this.getGrades(exam.gradeIds) : ''}
        ${(exam.improvementGradeIds && exam.improvementGradeIds.length) ? this.getGrades(exam.improvementGradeIds) : ''}
        ${(exam.classIds && exam.classIds.length > 0) ? this.getClasses(exam.classIds) : ''}
        ${(exam.improvementClassIds && exam.improvementClassIds.length > 0) ? this.getClasses(exam.improvementClassIds) : ''}
      `;
    return strValue.indexOf(filterValue) === -1;
  }

  getQuestionnaireInfo(exam: IExamDto) {
    if (!exam) {
      return '';
    }
    const questionnaire = this.questionnairesDic[exam.questionnaireId];
    if (!questionnaire) {
      return '';
    }
    return `(${questionnaire.code}) ${questionnaire.name}`;
  }

  getSubjectName(exam: IExamDto) {
    if (!exam) {
      return '';
    }
    const questionnaire = this.questionnairesDic[exam.questionnaireId];
    if (!questionnaire) {
      return '';
    }
    const subject = this.subjectsDic[questionnaire.subjectId];
    return subject?.name;
  }

  applyFilter(): void {
    this.filteredExams = [... this._exams.filter(e => !this.isFilteredExam(e, this.searchValue)).map(x => x)];
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

  getDay(isoDateTime: string) {
    if (!isoDateTime) {
      return '';
    }

    const date = new Date(isoDateTime);
    if (isNaN(date.getTime())) {
      return '';
    }

    // Get the day of the week (0 = Sunday, 6 = Saturday)
    const days = ['DAYS.SUNDAY', 'DAYS.MONDAY', 'DAYS.TUESDAY', 'DAYS.WEDNESDAY', 'DAYS.THURSDAY', 'DAYS.FRIDAY', 'DAYS.SATURDAY'];
    return days[date.getDay()];
  }

  onSetAsVisible(examId: string) {
    this._examsService.setAsVisible(examId).subscribe({
      next: examDto => {
        const index = this._exams.findIndex(x => x.id === examDto.id);
        if (index !== -1) {
          this._exams[index].isVisible = examDto.isVisible;
          this.applyFilter();
        }
      }
    })
  }

  onExport() {
    const days = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];

    const sheetData: IExportDataItem[] = this.filteredExams.map(x => {
      const examDate = new Date(x.startTime);
      const questionnaire = this.questionnairesDic[x.questionnaireId];
      return {
        examDay: days[examDate.getDay()],
        startTime: DateTimeUtils.getDateTimeFromIso(x.startTime, 'DD/MM/yyyy HH:mm'),
        subject: (questionnaire?.subjectId) ? this.subjectsDic[questionnaire?.subjectId].name : '',
        questionnaireName: questionnaire?.name,
        questionnaireCode: questionnaire?.code,
        examType: this.examTypesDic[x.examTypeId]?.name,
        moed: this.moedsDic[x.moedId]?.name,
        duration: x.duration,
        durationWithExtra: x.durationWithExtra,
        firstTime: [
          ...(x.gradeIds?.map(x => this.gradesDic[x].name) || []),
          ...(x.classIds?.map(x => this.classesDic[x].name) || [])
        ],
        improvement: [
          ...(x.improvementGradeIds?.map(x => this.gradesDic[x].name) || []),
          ...(x.improvementClassIds?.map(x => this.classesDic[x].name) || [])
        ],
      };
    });
    this._exportService.exportToExcel(sheetData);
  }
}
