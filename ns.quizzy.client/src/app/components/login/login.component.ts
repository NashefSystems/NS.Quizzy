import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { StorageService } from '../../services/storage.service';
import { ClientAppSettingsService } from '../../services/backend/client-app-settings.service';
import { AccountService } from '../../services/backend/account.service';
import { LoginRequest } from '../../models/backend/login.request';
import { NotificationsService } from '../../services/notifications.service';
import { LocalStorageKeys } from '../../enums/local-storage-keys.enum';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-login',
  standalone: false,

  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  private readonly _fb = inject(FormBuilder);
  private readonly _storageService = inject(StorageService);
  private readonly _clientAppSettingsService = inject(ClientAppSettingsService);
  private readonly _accountService = inject(AccountService);
  private readonly _notificationsService = inject(NotificationsService);
  private readonly _dialogRef = inject(MatDialogRef<LoginComponent>);

  hidePassword = true;
  loginForm: FormGroup = this._fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required]],
    rememberMe: [false]
  });

  ngOnInit(): void {
    let savedLoginInfo = this._storageService.getSensitiveLocalStorage(LocalStorageKeys.loginInfo);
    if (savedLoginInfo) {
      const { email, password } = savedLoginInfo;
      this.loginForm.patchValue({
        email: email,
        password: password,
        rememberMe: true
      });
    }
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      const { email, password, rememberMe } = this.loginForm.value;

      if (rememberMe) {
        const storageValue = { email: email, password: '' };
        this._clientAppSettingsService.get().subscribe({
          next: (data) => {
            if (data.SavePasswordOnRememberMe) {
              storageValue.password = password;
            }
            this._storageService.setSensitiveLocalStorage(LocalStorageKeys.loginInfo, storageValue);
          }
        });
      } else {
        // Clear local storage if remember me is unchecked
        this._storageService.removeLocalStorage(LocalStorageKeys.loginInfo);
      }

      const loginRequest: LoginRequest = {
        email: email,
        password: password
      };
      this._accountService.login(loginRequest).subscribe({
        next: (responseBody) => {
          this._notificationsService.success("LOGIN.LOGIN_SUCCESSFULLY",{fullName:responseBody.fullName});
          this._dialogRef.close();
        },
        error: (error) => {
          if (error?.status === 401) {
            this._notificationsService.error("LOGIN.INVALID_CREDENTIALS");
            return;
          }
          const msg = error?.message;
          this._notificationsService.fatal('UNEXPECTED_ERROR', { message: msg });
        }
      });
    }
  }

  togglePasswordVisibility(): void {
    this.hidePassword = !this.hidePassword;
  }
}
