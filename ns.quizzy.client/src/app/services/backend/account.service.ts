import { inject, Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { UserDetailsDto } from '../../models/backend/user-details.dto';
import { LoginRequest } from '../../models/backend/login.request';
import { BehaviorSubject, of, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {
  controllerName: string = 'Account';
  private userSubject = new BehaviorSubject<UserDetailsDto | null>(null);
  public userChange = this.userSubject.asObservable();

  constructor() {
    super();
    this.getDetails().subscribe();
  }

  getTest() {
    return this.httpClient.post<string>(`${this.getBaseUrl()}/test`, null);
  }

  login(request: LoginRequest) {
    return this.httpClient.post<UserDetailsDto>(`${this.getBaseUrl()}/Login`, request)
      .pipe(tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      }));
  }

  getDetails() {
    if (this.userSubject.value) {
      return of(this.userSubject.value);
    }
    return this.httpClient.get<UserDetailsDto>(`${this.getBaseUrl()}/Details`)
      .pipe(tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      }));
  }

  logout() {
    return this.httpClient.delete(`${this.getBaseUrl()}/Logout`).pipe(tap({
      next: () => this.userSubject.next(null)
    }));
  }

  isAuthenticatedUser() {
    return !!(this.userSubject.value?.id);
  }
}
