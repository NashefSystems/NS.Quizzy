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
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatMenuModule } from '@angular/material/menu';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule, MatInputModule, MatCheckboxModule, MatCardModule,
  MatExpansionModule, MatProgressBarModule, MatSidenavModule, MatMenuModule, MatSnackBarModule,
  MatTableModule, MatPaginatorModule,
];

import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { ExamsComponent } from './components/exams/exams.component';
import { RootComponent } from './components/layout/root/root.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { PrettyJsonPipe } from './pipes/pretty-json.pipe';
import { HeaderComponent } from './components/layout/header/header.component';
import { FilterDialogComponent } from './components/exams/filter-dialog/filter-dialog.component';
import { CheckboxTreeComponent } from './components/checkbox-tree/checkbox-tree.component';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';
import { MainMenuComponent } from './components/main-menu/main-menu.component';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { NotificationSnackBarComponent } from './components/notification-snack-bar/notification-snack-bar.component';
import { TestComponent } from './components/test/test.component';
import { ExamListComponent } from './components/exam-area/exam-list/exam-list.component';
import { ClassListComponent } from './components/class-area/class-list/class-list.component';
import { ExamTypeListComponent } from './components/exam-type-area/exam-type-list/exam-type-list.component';
import { SubjectListComponent } from './components/subject-area/subject-list/subject-list.component';
import { TableComponent } from './components/table/table.component';
import { QRCodeComponent } from 'angularx-qrcode';
import { SubjectAddOrEditComponent } from './components/subject-area/subject-add-or-edit/subject-add-or-edit.component';
import { ConfirmDialogComponent } from './components/confirm-dialog/confirm-dialog.component';
import { BackButtonComponent } from './components/back-button/back-button.component';
import { ExamTypeAddOrEditComponent } from './components/exam-type-area/exam-type-add-or-edit/exam-type-add-or-edit.component';


// Factory function for HttpLoader
export function HttpLoaderFactory(http: HttpClient) {
  return new TranslateHttpLoader(http, './assets/i18n/', '.json');
}

@NgModule({
  declarations: [
    RootComponent,
    ExamsComponent,
    HeaderComponent,
    FilterDialogComponent,
    PrettyJsonPipe,
    CheckboxTreeComponent,
    LoginComponent,
    LogoutComponent,
    MainMenuComponent,
    LanguageSelectorComponent,
    NotificationSnackBarComponent,
    TestComponent,
    ExamListComponent,
    ClassListComponent,
    ExamTypeListComponent,
    SubjectListComponent,
    TableComponent,
    SubjectAddOrEditComponent,
    ConfirmDialogComponent,
    BackButtonComponent,
    ExamTypeAddOrEditComponent,
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
  ],
  providers: [
    provideHttpClient(),
    provideAnimationsAsync(),
  ],
  bootstrap: [
    RootComponent
  ]
})
export class AppModule { }
