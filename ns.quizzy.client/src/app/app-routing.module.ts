import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamsComponent } from './components/exams/exams.component';
import { ExamListComponent } from './components/exam-area/exam-list/exam-list.component';
import { QuestionnaireListComponent } from './components/questionnaire-area/questionnaire-list/questionnaire-list.component';
import { ExamTypeListComponent } from './components/exam-type-area/exam-type-list/exam-type-list.component';
import { SubjectListComponent } from './components/subject-area/subject-list/subject-list.component';
import { ClassListComponent } from './components/class-area/class-list/class-list.component';
import { TestComponent } from './components/test/test.component';
import { adminUserGuard } from './guards/admin-user.guard';
import { SubjectAddOrEditComponent } from './components/subject-area/subject-add-or-edit/subject-add-or-edit.component';
import { ExamTypeAddOrEditComponent } from './components/exam-type-area/exam-type-add-or-edit/exam-type-add-or-edit.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ExamsComponent,
  },
  {
    path: 'exams',
    component: ExamListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'classes',
    component: ClassListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'questionnaires',
    component: QuestionnaireListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'exam-types',
    component: ExamTypeListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'exam-types/new',
    component: ExamTypeAddOrEditComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'exam-types/edit/:id',
    component: ExamTypeAddOrEditComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'subjects',
    component: SubjectListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'subjects/new',
    component: SubjectAddOrEditComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'subjects/edit/:id',
    component: SubjectAddOrEditComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'test',
    component: TestComponent,
  },
  {
    path: '**',
    pathMatch: 'full',
    redirectTo: '/',
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
