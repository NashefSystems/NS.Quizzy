import { inject, Injectable } from '@angular/core';
import { BaseService } from './base.service';
import { UserDetailsDto } from '../../models/backend/user-details.dto';
import { LoginRequest } from '../../models/backend/login.request';
import { LoginResponse } from '../../models/backend/login.response';
import { BehaviorSubject, catchError, from, map, mergeMap, Observable, of, shareReplay, tap } from 'rxjs';
import { VerifyOTPRequest } from '../../models/backend/verify-otp.request';
import { CookieService } from 'ngx-cookie-service';
import { LoginWithIdNumberRequest } from '../../models/backend/login-with-id-number.request';
import { WebviewBridgeService } from '../webview-bridge.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService extends BaseService {
  private readonly _webviewBridgeService = inject(WebviewBridgeService);
  controllerName: string = 'Account';
  private userSubject = new BehaviorSubject<UserDetailsDto | null>(null);
  public userChange = this.userSubject.asObservable();
  private details$: Observable<UserDetailsDto | null> | null = null;

  constructor(
    private readonly cookieService: CookieService,
  ) {
    super();
    this.getDetails().subscribe();
  }

  getTest() {
    return this.httpClient.post<string>(`${this.getBaseUrl()}/test`, null);
  }

  login(request: LoginRequest): Observable<LoginResponse> {
    this.userSubject.next(null);
    request.deviceId = null;
    request.notificationToken = null;

    return from(this._webviewBridgeService.getMobileSerialNumberAsync()).pipe(
      catchError(() => of(null)), // handle error silently
      mergeMap(res => {
        const did = res?.uniqueId ?? res?.serialNumber;
        if (did) {
          request.deviceId = did;
        }

        return from(this._webviewBridgeService.getNotificationTokenAsync()).pipe(
          catchError(() => of(null)),
          mergeMap(tokenRes => {
            if (tokenRes?.token) {
              request.notificationToken = tokenRes.token;
            }
            debugger;
            return this.httpClient.post<LoginResponse>(`${this.getBaseUrl()}/Login`, request);
          })
        );
      })
    );
  }

  loginWithIdNumber(request: LoginWithIdNumberRequest): Observable<UserDetailsDto> {
    this.userSubject.next(null);
    request.deviceId = null;
    request.notificationToken = null;

    return from(this._webviewBridgeService.getMobileSerialNumberAsync()).pipe(
      catchError(() => of(null)),
      mergeMap(res => {
        const did = res?.uniqueId ?? res?.serialNumber;
        if (did) {
          request.deviceId = did;
        }

        return from(this._webviewBridgeService.getNotificationTokenAsync()).pipe(
          catchError(() => of(null)),
          mergeMap(tokenRes => {
            if (tokenRes?.token) {
              request.notificationToken = tokenRes.token;
            }

            return this.httpClient.post<UserDetailsDto>(`${this.getBaseUrl()}/LoginWithIdNumber`, request).pipe(
              tap({
                next: (data) => this.userSubject.next(data),
                error: () => this.userSubject.next(null),
              })
            );
          })
        );
      })
    );
  }


  verifyOTP(request: VerifyOTPRequest) {
    return this.httpClient.post<UserDetailsDto>(`${this.getBaseUrl()}/VerifyOTP`, request)
      .pipe(tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      }));
  }

  tokenIsExists() {
    return this.cookieService.check('_qat');
  }

  getDetails(): Observable<UserDetailsDto | null> {
    if (!this.tokenIsExists()) {
      return of(null);
    }

    if (this.userSubject.value) {
      return of(this.userSubject.value);
    }

    if (this.details$) {
      return this.details$;
    }

    this.details$ = this.httpClient.get<UserDetailsDto>(`${this.getBaseUrl()}/Details`).pipe(
      tap({
        next: (data) => this.userSubject.next(data),
        error: () => this.userSubject.next(null),
      }),
      map(data => data ?? null), // normalize undefined to null
      shareReplay(1) // ✅ cache the result
    );

    return this.details$;
  }

  logout() {
    this.details$ = null; // 🔄 reset cache
    return this.httpClient.delete(`${this.getBaseUrl()}/Logout`).pipe(tap({
      next: () => this.userSubject.next(null)
    }));
  }
}
