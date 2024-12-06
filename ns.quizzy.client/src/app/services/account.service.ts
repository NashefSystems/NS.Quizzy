import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  readonly baseUrl = '/api/account';
  constructor(
    private readonly http: HttpClient
  ) { }

  getTest() {
    return this.http.post<string>(`${this.baseUrl}/test`, null);
  }

}
