import { Component, EventEmitter, inject, Input, OnInit, viewChild } from '@angular/core';
import { IExamDto } from '../../../models/backend/exam.dto';
import { IGradeDto } from '../../../models/backend/grade.dto';
import { IClassDto } from '../../../models/backend/class.dto';
import { IQuestionnaireDto } from '../../../models/backend/questionnaire.dto';
import { IExamTypeDto } from '../../../models/backend/exam-type.dto';
import { ISubjectDto } from '../../../models/backend/subject.dto';
import { MatAccordion } from '@angular/material/expansion';
import { AppTranslateService } from '../../../services/app-translate.service';
import { IMoedDto } from '../../../models/backend/moed.dto';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';
import { ExportDataItem, ExportService } from './export.service';
import { DateTimeUtils } from '../../../utils/date-time.utils';
import { WebviewBridgeService } from '../../../services/webview-bridge.service';

@Component({
  selector: 'app-exam-schedule-list',
  standalone: false,
  templateUrl: './exam-schedule-list.component.html',
  styleUrl: './exam-schedule-list.component.scss'
})
export class ExamScheduleListComponent implements OnInit {
  private readonly _webviewBridgeService = inject(WebviewBridgeService);

  isLoading: boolean = true;
  iconColor: string = '#0053E7';
  appVersion: string = "";
  nativeAppIsAvailable: boolean | null = null;

  @Input() exams: IExamDto[] = [];
  @Input() grades: IGradeDto[] = [];
  @Input() classes: IClassDto[] = [];
  @Input() questionnaires: IQuestionnaireDto[] = [];
  @Input() examTypes: IExamTypeDto[] = [];
  @Input() moeds: IMoedDto[] = [];
  @Input() subjects: ISubjectDto[] = [];
  @Input() filterIsActive: boolean;
  @Input() onFilter = new EventEmitter<void>();

  gradesDic: { [key: string]: IGradeDto } = {};
  classesDic: { [key: string]: IClassDto } = {};
  questionnairesDic: { [key: string]: IQuestionnaireDto } = {};
  examTypesDic: { [key: string]: IExamTypeDto } = {};
  moedsDic: { [key: string]: IMoedDto } = {};
  subjectsDic: { [key: string]: ISubjectDto } = {};
  accordion = viewChild.required(MatAccordion);

  private readonly _appTranslateService = inject(AppTranslateService);
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);
  ExportService: any;

  ngOnInit(): void {
    this.nativeAppIsAvailable = this._webviewBridgeService.nativeAppIsAvailable();
    this._clientAppSettingsService.get().subscribe({ next: result => this.appVersion = result?.AppVersion })
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

    const dir = this._appTranslateService.translate("DIR");
    const title = this._appTranslateService.translate("PAGE_TITLES.EXAM_SCHEDULE");

    // Create an iframe for printing
    const printFrame = document.createElement('iframe');
    printFrame.style.position = 'absolute';
    printFrame.style.width = '0px';
    printFrame.style.height = '0px';
    printFrame.style.border = 'none';
    document.body.appendChild(printFrame);

    const printDoc = printFrame.contentWindow?.document;
    if (!printDoc) {
      console.error('Failed to create print document');
      return;
    }

    printDoc.open();
    printDoc.write(`
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
            .app-info {
                margin-top: 1rem;
                display: flex;
                justify-content: center;
                color: gray;
                direction: ltr;
            }
          </style>
        </head>
        <body dir="${dir}">
          <h1 class="page-title">${title}</h1>            
          ${printContent}
          <div class="app-info">
              <small>Quizzy App (V${this.appVersion})</small>
          </div>
        </body>
      </html>
    `);
    printDoc.close();

    printFrame.contentWindow?.focus();
    printFrame.contentWindow?.print();

    // Remove the iframe after printing (wait a bit to ensure printing starts)
    setTimeout(() => {
      document.body.removeChild(printFrame);
    }, 500);
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

  onExport() {
    const days = ["ראשון", "שני", "שלישי", "רביעי", "חמישי", "שישי", "שבת"];

    const sheetData: ExportDataItem[] = this.exams.map(x => {
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
    ExportService.exportToExcel(sheetData);
  }
}
