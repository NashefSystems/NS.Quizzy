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
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule, MatInputModule, MatCheckboxModule, MatCardModule,
  MatExpansionModule, MatProgressBarModule, MatSidenavModule, MatMenuModule, MatSnackBarModule
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
import { QuestionnaireListComponent } from './components/questionnaires/questionnaire-list/questionnaire-list.component';
import { QuestionnaireInsertOrUpdateComponent } from './components/questionnaires/questionnaire-insert-or-update/questionnaire-insert-or-update.component';
import { LoginComponent } from './components/login/login.component';
import { LogoutComponent } from './components/logout/logout.component';
import { MainMenuComponent } from './components/main-menu/main-menu.component';
import { LanguageSelectorComponent } from './components/language-selector/language-selector.component';
import { NotificationSnackBarComponent } from './components/notification-snack-bar/notification-snack-bar.component';
import { TestComponent } from './components/test/test.component';


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
    QuestionnaireListComponent,
    QuestionnaireInsertOrUpdateComponent,
    LoginComponent,
    LogoutComponent,
    MainMenuComponent,
    LanguageSelectorComponent,
    NotificationSnackBarComponent,
    TestComponent,
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
