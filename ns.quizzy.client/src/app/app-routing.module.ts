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

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ExamsComponent,
  },
  {
    path: 'exam-list',
    component: ExamListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'class-list',
    component: ClassListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'questionnaire-list',
    component: QuestionnaireListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'exam-type-list',
    component: ExamTypeListComponent,
    canActivate: [adminUserGuard]
  },
  {
    path: 'subject-list',
    component: SubjectListComponent,
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
