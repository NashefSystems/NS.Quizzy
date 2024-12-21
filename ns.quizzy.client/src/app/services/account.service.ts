import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { BackendBaseService } from './backend-base.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BackendBaseService {
  controllerName: string = 'account';

  getTest() {
    return this.http.post<string>(`${this.getBaseUrl()}/test`, null);
  }

}
