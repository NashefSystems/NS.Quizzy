import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StorageService } from '../../../services/storage.service';
import { ClientAppSettingsService } from '../../../services/backend/client-app-settings.service';
import { AccountService } from '../../../services/backend/account.service';
import { LoginRequest } from '../../../models/backend/login.request';
import { NotificationsService } from '../../../services/notifications.service';
import { LocalStorageKeys } from '../../../enums/local-storage-keys.enum';
import { LoginSteps } from './login-steps.enum'
import { LoginResponse } from '../../../models/backend/login.response';
import { VerifyOTPRequest } from '../../../models/backend/verify-otp.request';
import { Router } from '@angular/router';
import { AppSettingsService } from '../../../services/app-settings.service';
import { GlobalService } from '../../../services/global.service';

@Component({
  selector: 'app-login-with-email',
  standalone: false,

  templateUrl: './login-with-email.component.html',
  styleUrl: './login-with-email.component.scss'
})
export class LoginWithEmailComponent implements OnInit {
  private readonly localStorageKey = LocalStorageKeys.loginWithIdNumberInfo;
  private readonly _fb = inject(FormBuilder);
  private readonly _globalService = inject(GlobalService);
  private readonly _storageService = inject(StorageService);
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);
  private readonly _appSettingsService = inject(AppSettingsService);
  private readonly _accountService = inject(AccountService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);

  LoginSteps = LoginSteps;
  step = LoginSteps.UserNameAndPassword;
  loginResponse: LoginResponse | null = null;

  hidePassword = true;
  loginForm: FormGroup = this._fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [true],
  });

  twoFactorForm: FormGroup = this._fb.group({
    token: ['', [Validators.required]]
  });

  ngOnInit(): void {
    let savedLoginInfo = this._storageService.getSensitiveLocalStorage(this.localStorageKey);
    if (savedLoginInfo) {
      const { email, password } = savedLoginInfo;
      this.loginForm.patchValue({
        email: email,
        password: password,
        rememberMe: true
      });
    }
  }

  onLoginSubmit(): void {
    if (!this.loginForm.valid) {
      return;
    }
    const { email, password, rememberMe } = this.loginForm.value;

    if (rememberMe) {
      const storageValue = { email: email, password: '' };
      this._clientAppSettingsService.get().subscribe({
        next: (data) => {
          if (data.SavePasswordOnRememberMe) {
            storageValue.password = password;
          }
          this._storageService.setSensitiveLocalStorage(this.localStorageKey, storageValue);
        }
      });
    } else {
      // Clear local storage if remember me is unchecked
      this._storageService.removeLocalStorage(this.localStorageKey);
    }

    const loginRequest: LoginRequest = {
      email: email,
      password: password
    };
    this._accountService.login(loginRequest).subscribe({
      next: (responseBody) => {
        this._globalService.updateDeviceInfoAsync();
        this.loginResponse = responseBody;
        if (responseBody.requiresTwoFactor) {
          this.step = LoginSteps.OTP;
        } else {
          this._accountService.getDetails().subscribe({
            next: (data) => {
              this._notificationsService.success("LOGIN.LOGIN_SUCCESSFULLY", { fullName: data?.fullName });
              this._router.navigate([this._appSettingsService.homeUrl]);
            }
          })
        }
      },
      error: (error) => {
        if (error?.status === 401) {
          this._notificationsService.error("LOGIN.INVALID_CREDENTIALS");
          return;
        }
        this._notificationsService.httpErrorHandler(error);
      }
    });
  }

  onVerifyOTP() {
    if (!this.loginForm.valid) {
      return;
    }
    const { token } = this.twoFactorForm.value;
    const request: VerifyOTPRequest = {
      id: this.loginResponse?.contextId ?? '',
      token: token,
    };
    this._accountService.verifyOTP(request).subscribe({
      next: (responseBody) => {
        this._notificationsService.success("LOGIN.LOGIN_SUCCESSFULLY", { fullName: responseBody.fullName });
        this._router.navigate([this._appSettingsService.homeUrl]);
      },
      error: (error) => {
        this._notificationsService.error('LOGIN.TWO_FACTOR.ERROR');
      }
    });
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }
}
