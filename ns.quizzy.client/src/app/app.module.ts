import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { ExamsComponent } from './coponents/exams/exams.component';
import { RootComponent } from './coponents/root/root.component';

@NgModule({
  declarations: [
    ExamsComponent,
    RootComponent
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [RootComponent]
})
export class AppModule { }
