import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AccountService } from '../../../services/backend/account.service';
import { NotificationsService } from '../../../services/notifications.service';
import { Router } from '@angular/router';
import { AppSettingsService } from '../../../services/app-settings.service';
import { LoginWithIdNumberRequest } from '../../../models/backend/login-with-id-number.request';
import { StorageService } from '../../../services/storage.service';
import { LocalStorageKeys } from '../../../enums/local-storage-keys.enum';

@Component({
  selector: 'app-login-with-id-number',
  standalone: false,

  templateUrl: './login-with-id-number.component.html',
  styleUrl: './login-with-id-number.component.scss'
})
export class LoginWithIdNumberComponent implements OnInit {
  private readonly localStorageKey = LocalStorageKeys.loginWithIdNumberInfo;
  private readonly _fb = inject(FormBuilder);
  private readonly _appSettingsService = inject(AppSettingsService);
  private readonly _accountService = inject(AccountService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _router = inject(Router);
  private readonly _storageService = inject(StorageService);

  loginForm: FormGroup = this._fb.group({
    idNumber: ['', [Validators.required]],
    rememberMe: [true],
  });

  ngOnInit(): void {
    let savedLoginInfo = this._storageService.getSensitiveLocalStorage(this.localStorageKey);
    if (savedLoginInfo) {
      const { idNumber } = savedLoginInfo;
      this.loginForm.patchValue({
        idNumber: idNumber,
        rememberMe: true
      });
    }
  }

  onLoginSubmit(): void {
    if (!this.loginForm.valid) {
      return;
    }
    const { idNumber, rememberMe } = this.loginForm.value;

    if (rememberMe) {
      const storageValue = { idNumber: idNumber };
      this._storageService.setSensitiveLocalStorage(this.localStorageKey, storageValue);
    } else {
      // Clear local storage if remember me is unchecked
      this._storageService.removeLocalStorage(this.localStorageKey);
    }

    const loginRequest: LoginWithIdNumberRequest = {
      idNumber: idNumber
    };
    this._accountService.loginWithIdNumber(loginRequest).subscribe({
      next: (data) => {
        this._notificationsService.success("LOGIN.LOGIN_SUCCESSFULLY", { fullName: data?.fullName });
        this._router.navigate([this._appSettingsService.homeUrl]);
      },
      error: (error) => {
        if (error?.status === 401) {
          this._notificationsService.error("LOGIN.INVALID_ID_NUMBER");
          return;
        }
        this._notificationsService.httpErrorHandler(error);
      }
    });
  }
}
