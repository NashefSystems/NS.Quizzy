import { inject, Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { UserDetailsDto } from '../../models/backend/user-details.dto';
import { LoginRequest } from '../../models/backend/login.request';
import { LoginResponse } from '../../models/backend/login.response';
import { BehaviorSubject, finalize, from, of, tap } from 'rxjs';
import { VerifyOTPRequest } from '../../models/backend/verify-otp.request';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {
  controllerName: string = 'Account';
  private userSubject = new BehaviorSubject<UserDetailsDto | null>(null);
  public userChange = this.userSubject.asObservable();
  private pendingRequest: Promise<UserDetailsDto | undefined> | null = null;


  constructor() {
    super();
    this.getDetails().subscribe();
  }

  getTest() {
    return this.httpClient.post<string>(`${this.getBaseUrl()}/test`, null);
  }

  login(request: LoginRequest) {
    this.userSubject.next(null)
    return this.httpClient.post<LoginResponse>(`${this.getBaseUrl()}/Login`, request);
  }

  verifyOTP(request: VerifyOTPRequest) {
    return this.httpClient.post<UserDetailsDto>(`${this.getBaseUrl()}/VerifyOTP`, request)
      .pipe(tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      }));
  }

  getDetails() {
    if (this.userSubject.value) {
      // If user details are already cached, return them immediately
      return of(this.userSubject.value);
    }

    if (this.pendingRequest) {
      // If there's an ongoing request, return the same promise as an Observable
      return from(this.pendingRequest);
    }
    // Create a new request and store it in `pendingRequest`
    this.pendingRequest = this.httpClient.get<UserDetailsDto>(`${this.getBaseUrl()}/Details`).pipe(
      tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      })
    ).toPromise();

    return from(this.pendingRequest).pipe(
      finalize(() => {
        // Reset the pending request once it's completed
        this.pendingRequest = null;
      })
    );
  }

  logout() {
    return this.httpClient.delete(`${this.getBaseUrl()}/Logout`).pipe(tap({
      next: () => this.userSubject.next(null)
    }));
  }
}
