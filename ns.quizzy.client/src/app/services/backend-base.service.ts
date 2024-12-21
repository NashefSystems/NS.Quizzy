import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export abstract class BackendBaseService {
  protected readonly http = inject(HttpClient);
  protected abstract controllerName: string;

  getBaseUrl() {
    return `/api/${this.controllerName}`;
  };
}
