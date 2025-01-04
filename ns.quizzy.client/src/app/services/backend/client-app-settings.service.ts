import { Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { ClientAppSettingsResponse } from '../../models/backend/client-app-settings.response';
import { of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ClientAppSettingsService extends BaseService {
  controllerName: string = 'ClientAppSettings';
  clientAppSettingsResponseCache: ClientAppSettingsResponse | null = null;

  get() {
    if (this.clientAppSettingsResponseCache) {
      return of(this.clientAppSettingsResponseCache);
    }
    return this.httpClient.get<ClientAppSettingsResponse>(this.getBaseUrl())
      .pipe(tap({
        next: (data) => this.clientAppSettingsResponseCache = data
      }));
  }
}
