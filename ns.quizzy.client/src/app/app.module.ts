import { provideHttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { ExamsComponent } from './coponents/exams/exams.component';
import { RootComponent } from './coponents/root/root.component';
import { ExamListFilterComponent } from './coponents/exam-list-filter/exam-list-filter.component';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatDialogModule } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule
];

@NgModule({
  declarations: [
    ExamsComponent,
    RootComponent,
    ExamListFilterComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ...matModules
  ],
  providers: [
    provideHttpClient(),
  ],
  bootstrap: [RootComponent]
})
export class AppModule { }
