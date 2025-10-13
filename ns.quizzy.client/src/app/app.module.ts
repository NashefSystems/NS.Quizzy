import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatCardModule } from '@angular/material/card';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatBadgeModule } from '@angular/material/badge';
import { MatRadioModule } from '@angular/material/radio';
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule, MatInputModule, MatCheckboxModule, MatCardModule,
  MatExpansionModule, MatProgressBarModule, MatProgressSpinner, MatSidenavModule, MatMenuModule,
  MatSnackBarModule, MatTableModule, MatPaginatorModule, MatSelectModule, MatTabsModule, MatTooltipModule,
  MatBadgeModule, MatRadioModule
];

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { RootComponent } from './components/layout-area/root/root.component';
import { HeaderComponent } from './components/layout-area/header/header.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { PrettyJsonPipe } from './pipes/pretty-json.pipe';
import { LoginComponent } from './components/auth-area/login/login.component';
import { MainMenuComponent } from './components/global-area/main-menu/main-menu.component';
import { LanguageSelectorComponent } from './components/global-area/language-selector/language-selector.component';
import { NotificationSnackBarComponent } from './components/global-area/notification-snack-bar/notification-snack-bar.component';
import { ExamListComponent } from './components/exam-area/exam-list/exam-list.component';
import { ExamTypeListComponent } from './components/exam-type-area/exam-type-list/exam-type-list.component';
import { SubjectListComponent } from './components/subject-area/subject-list/subject-list.component';
import { TableComponent } from './components/global-area/table/table.component';
import { QRCodeComponent } from 'angularx-qrcode';
import { SubjectAddOrEditComponent } from './components/subject-area/subject-add-or-edit/subject-add-or-edit.component';
import { ConfirmDialogComponent } from './components/global-area/confirm-dialog/confirm-dialog.component';
import { BackButtonComponent } from './components/global-area/back-button/back-button.component';
import { ExamTypeAddOrEditComponent } from './components/exam-type-area/exam-type-add-or-edit/exam-type-add-or-edit.component';
import { ClassListComponent } from './components/class-area/class-list/class-list.component';
import { ClassAddOrEditComponent } from './components/class-area/class-add-or-edit/class-add-or-edit.component';
import { GradeListComponent } from './components/grade-area/grade-list/grade-list.component';
import { GradeAddOrEditComponent } from './components/grade-area/grade-add-or-edit/grade-add-or-edit.component';
import { QuestionnaireAddOrEditComponent } from './components/questionnaire-area/questionnaire-add-or-edit/questionnaire-add-or-edit.component';
import { QuestionnaireListComponent } from './components/questionnaire-area/questionnaire-list/questionnaire-list.component';
import { ExamScheduleListComponent } from './components/exam-schedule-area/exam-schedule-list/exam-schedule-list.component';
import { ExamScheduleHomeComponent } from './components/exam-schedule-area/exam-schedule-home/exam-schedule-home.component';
import { ExamScheduleCalendarComponent } from './components/exam-schedule-area/exam-schedule-calendar/exam-schedule-calendar.component';
import { ExamAddOrEditComponent } from './components/exam-area/exam-add-or-edit/exam-add-or-edit.component';
import { ExamScheduleFilterComponent } from './components/exam-schedule-area/exam-schedule-filter/exam-schedule-filter.component';
import { ExamInfoValueComponent } from './components/exam-schedule-area/exam-schedule-list/exam-info-value/exam-info-value.component';
import { TimePipe } from './pipes/time.pipe';
import { CamelToSnakePipe } from './pipes/camel-to-snake.pipe';
import { LoadingInterceptor } from './interceptors/loading.interceptor';
import { MoedListComponent } from './components/moed-area/moed-list/moed-list.component';
import { MoedAddOrEditComponent } from './components/moed-area/moed-add-or-edit/moed-add-or-edit.component';
import { SkeletonLoaderComponent } from './components/global-area/skeleton-loader/skeleton-loader.component';
import { PrivacyPolicyComponent } from './components/global-area/privacy-policy/privacy-policy.component';
import { LoginWithIdNumberComponent } from './components/auth-area/login-with-id-number/login-with-id-number.component';
import { LoginWithEmailComponent } from './components/auth-area/login-with-email/login-with-email.component';
import { UserDetailsComponent } from './components/auth-area/user-details/user-details.component';
import { UserListComponent } from './components/user-area/user-list/user-list.component';
import { UserAddOrEditComponent } from './components/user-area/user-add-or-edit/user-add-or-edit.component';
import { MyNotificationsComponent } from './components/notification-area/my-notifications/my-notifications.component';
import { NotificationListComponent } from './components/notification-area/notification-list/notification-list.component';
import { NotificationAddComponent } from './components/notification-area/notification-add/notification-add.component';
import { NotificationGroupListComponent } from './components/notification-group-area/notification-group-list/notification-group-list.component';
import { NotificationGroupAddOrEditComponent } from './components/notification-group-area/notification-group-add-or-edit/notification-group-add-or-edit.component';
import { NotificationTemplateListComponent } from './components/notification-template-area/notification-template-list/notification-template-list.component';
import { NotificationTemplateAddOrEditComponent } from './components/notification-template-area/notification-template-add-or-edit/notification-template-add-or-edit.component';

import { DatePipe } from '@angular/common';
import { NgxMatSelectSearchModule } from 'ngx-mat-select-search';
import { FullLoadingComponent } from './components/global-area/full-loading/full-loading.component';
import { ForceUpdateComponent } from './components/global-area/force-update/force-update.component';
import { SelectNotificationTemplateDialogComponent } from './components/notification-template-area/select-notification-template-dialog/select-notification-template-dialog.component';

// Factory function for HttpLoader
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    BackButtonComponent,
    CamelToSnakePipe,
    ClassAddOrEditComponent,
    ClassListComponent,
    ConfirmDialogComponent,
    ExamAddOrEditComponent,
    ExamInfoValueComponent,
    ExamListComponent,
    ExamScheduleCalendarComponent,
    ExamScheduleFilterComponent,
    ExamScheduleHomeComponent,
    ExamScheduleListComponent,
    ExamTypeAddOrEditComponent,
    ExamTypeListComponent,
    GradeAddOrEditComponent,
    GradeListComponent,
    HeaderComponent,
    LanguageSelectorComponent,
    LoginComponent,
    LoginWithEmailComponent,
    LoginWithIdNumberComponent,
    MainMenuComponent,
    MoedAddOrEditComponent,
    MoedListComponent,
    MyNotificationsComponent,
    NotificationAddComponent,
    NotificationGroupListComponent,
    NotificationGroupAddOrEditComponent,
    NotificationTemplateListComponent,
    NotificationTemplateAddOrEditComponent,
    SelectNotificationTemplateDialogComponent,
    NotificationListComponent,
    NotificationSnackBarComponent,
    PrettyJsonPipe,
    PrivacyPolicyComponent,
    QuestionnaireAddOrEditComponent,
    QuestionnaireListComponent,
    RootComponent,
    SubjectAddOrEditComponent,
    SubjectListComponent,
    TableComponent,
    TimePipe,
    UserAddOrEditComponent,
    UserDetailsComponent,
    UserListComponent,
    FullLoadingComponent,
    ForceUpdateComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    ...matModules,
    NgxJsonViewerModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    }),
    QRCodeComponent,
    SkeletonLoaderComponent,
    NgxMatSelectSearchModule,
  ],
  providers: [
    provideHttpClient(withInterceptors([LoadingInterceptor])),
    provideAnimationsAsync(),
    TimePipe,
    CamelToSnakePipe,
    DatePipe,
  ],
  bootstrap: [
    RootComponent
  ]
})
export class AppModule { }
