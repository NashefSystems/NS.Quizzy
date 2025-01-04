import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export abstract class BaseService {
  protected readonly httpClient = inject(HttpClient);
  protected abstract controllerName: string;

  getBaseUrl() {
    return `/api/${this.controllerName}`;
  };
}
