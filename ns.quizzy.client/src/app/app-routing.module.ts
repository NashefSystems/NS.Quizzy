import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ExamsComponent } from './coponents/exams/exams.component';

const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    component: ExamsComponent
  }, {
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
