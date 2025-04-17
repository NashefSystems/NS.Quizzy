import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { DeveloperComponent } from './developer/developer.component';
import { ReactNativeMessageTestComponent } from './react-native-message-test/react-native-message-test.component';
import { TranslateModule } from '@ngx-translate/core';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';

const routes: Routes = [
  {
    path: '',
    children: [
      {
        path: 'react-native-message-test',
        component: ReactNativeMessageTestComponent
      },
      {
        path: '',
        pathMatch: 'full',
        component: DeveloperComponent
      },
      {
        path: '**',
        pathMatch: 'full',
        redirectTo: '/',
      }
    ]
  }
];


@NgModule({
  declarations: [
    DeveloperComponent,
    ReactNativeMessageTestComponent
  ],
  imports: [
    CommonModule,
    RouterModule.forChild(routes),
    TranslateModule, // ✅ just import this, no forRoot()
    NgxJsonViewerModule,
    MatButtonModule,
    MatIconModule
  ],
  exports: [RouterModule]
})
export class DeveloperModule { }
