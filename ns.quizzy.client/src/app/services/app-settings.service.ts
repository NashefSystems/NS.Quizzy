import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  readonly appMaxWidth = 420; // in px
}
