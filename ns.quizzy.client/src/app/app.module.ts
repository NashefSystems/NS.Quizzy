import { provideHttpClient } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { ExamsComponent } from './coponents/exams/exams.component';
import { RootComponent } from './coponents/layout/root/root.component';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { PrettyJsonPipe } from './pipes/pretty-json.pipe';
import { HeaderComponent } from './coponents/layout/header/header.component';
import { FilterDialogComponent } from './coponents/exams/filter-dialog/filter-dialog.component';
import { CheckboxTreeComponent } from './coponents/checkbox-tree/checkbox-tree.component';
import { LoaderComponent } from './coponents/layout/loader/loader.component';

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
const matModules = [
  MatFormFieldModule, MatChipsModule, MatIconModule, MatAutocompleteModule,
  MatDialogModule, MatButtonModule, MatInputModule, MatCheckboxModule, MatCardModule,
  MatExpansionModule, MatProgressBarModule, MatSidenavModule
];

@NgModule({
  declarations: [
    ExamsComponent,
    RootComponent,
    HeaderComponent,
    FilterDialogComponent,
    PrettyJsonPipe,
    CheckboxTreeComponent,
    LoaderComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    ...matModules,
    NgxJsonViewerModule
  ],
  providers: [
    provideHttpClient(),
  ],
  bootstrap: [
    RootComponent
  ]
})
export class AppModule { }
