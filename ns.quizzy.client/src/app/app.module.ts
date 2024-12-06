import { provideHttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { ExamsComponent } from './coponents/exams/exams.component';
import { RootComponent } from './coponents/layout/root/root.component';
import { ExamListFilterComponent } from './coponents/exam-list-filter/exam-list-filter.component';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatInputModule } from '@angular/material/input';
import { HeaderComponent } from './coponents/layout/header/header.component';
import { TestDialogComponent } from './coponents/dialogs/test-dialog/test-dialog.component';
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule, MatInputModule
];

@NgModule({
  declarations: [
    ExamsComponent,
    RootComponent,
    ExamListFilterComponent,
    HeaderComponent,
    TestDialogComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ...matModules
  ],
  providers: [
    provideHttpClient(),
  ],
  bootstrap: [
    RootComponent
  ]
})
export class AppModule { }
