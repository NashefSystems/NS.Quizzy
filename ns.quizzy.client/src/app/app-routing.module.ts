import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamListComponent } from './components/exam-area/exam-list/exam-list.component';
import { QuestionnaireListComponent } from './components/questionnaire-area/questionnaire-list/questionnaire-list.component';
import { ExamTypeListComponent } from './components/exam-type-area/exam-type-list/exam-type-list.component';
import { SubjectListComponent } from './components/subject-area/subject-list/subject-list.component';
import { ClassListComponent } from './components/class-area/class-list/class-list.component';
import { TestComponent } from './components/test/test.component';
import { adminUserGuard } from './guards/admin-user.guard';
import { SubjectAddOrEditComponent } from './components/subject-area/subject-add-or-edit/subject-add-or-edit.component';
import { ExamTypeAddOrEditComponent } from './components/exam-type-area/exam-type-add-or-edit/exam-type-add-or-edit.component';
import { ClassAddOrEditComponent } from './components/class-area/class-add-or-edit/class-add-or-edit.component';
import { GradeListComponent } from './components/grade-area/grade-list/grade-list.component';
import { GradeAddOrEditComponent } from './components/grade-area/grade-add-or-edit/grade-add-or-edit.component';
import { QuestionnaireAddOrEditComponent } from './components/questionnaire-area/questionnaire-add-or-edit/questionnaire-add-or-edit.component';
import { ExamScheduleHomeComponent } from './components/exam-schedule-area/exam-schedule-home/exam-schedule-home.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ExamScheduleHomeComponent,
    data: {
      page_title: "PAGE_TITLES.EXAM_SCHEDULE"
    }
  },
  {
    path: 'exams',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: ExamListComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_LIST"
        }
      }, {
        path: 'new',
        component: TestComponent,
        // component: ExamAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: TestComponent,
        // component: ExamAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/exams',
      }
    ]
  },
  {
    path: 'grades',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: GradeListComponent,
        data: {
          page_title: "PAGE_TITLES.GRADE_LIST"
        }
      }, {
        path: 'new',
        component: GradeAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.GRADE_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: GradeAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.GRADE_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/grades',
      }
    ]
  },
  {
    path: 'classes',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: ClassListComponent,
        data: {
          page_title: "PAGE_TITLES.CLASS_LIST"
        }
      }, {
        path: 'new',
        component: ClassAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.CLASS_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: ClassAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.CLASS_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/classes',
      }
    ]
  },
  {
    path: 'questionnaires',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: QuestionnaireListComponent,
        data: {
          page_title: "PAGE_TITLES.QUESTIONNAIRE_LIST"
        }
      }, {
        path: 'new',
        component: QuestionnaireAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.QUESTIONNAIRE_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: QuestionnaireAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.QUESTIONNAIRE_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/questionnaires',
      }
    ]
  },
  {
    path: 'exam-types',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: ExamTypeListComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_TYPE_LIST"
        }
      }, {
        path: 'new',
        component: ExamTypeAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_TYPE_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: ExamTypeAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.EXAM_TYPE_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/exam-types',
      }
    ]
  },
  {
    path: 'subjects',
    canActivate: [adminUserGuard],
    children: [
      {
        path: '',
        pathMatch: 'full',
        component: SubjectListComponent,
        data: {
          page_title: "PAGE_TITLES.SUBJECT_LIST"
        }
      }, {
        path: 'new',
        component: SubjectAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.SUBJECT_ADD"
        }
      },
      {
        path: 'edit/:id',
        component: SubjectAddOrEditComponent,
        data: {
          page_title: "PAGE_TITLES.SUBJECT_EDIT"
        }
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/subjects',
      }
    ]
  },
  {
    path: 'developer',
    component: TestComponent,
    data: {
      page_title: "Developer"
    }
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
