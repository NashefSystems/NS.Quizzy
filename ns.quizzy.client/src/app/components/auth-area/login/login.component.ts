import { Component, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LocalStorageKeys } from '../../../enums/local-storage-keys.enum';
import { StorageService } from '../../../services/storage.service';

enum LoginMethod {
  ID = 'ID',
  EMAIL = 'Email',
}

@Component({
  selector: 'app-login',
  standalone: false,
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent implements OnInit {
  LoginMethod = LoginMethod;
  private readonly localStorageKey = LocalStorageKeys.loginMethod;
  private readonly _storageService = inject(StorageService);
  private readonly _router = inject(Router);
  loginMethod: LoginMethod | null = null;

  constructor() {
    this.loginMethod = this._storageService.getLocalStorage(this.localStorageKey, LoginMethod.ID);
  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }

  onPrivacyPolicyClick() {
    this._router.navigate(['/privacy-policy']);
  }

  loginMethodToggle() {
    this.loginMethod = this.loginMethod === LoginMethod.ID ? LoginMethod.EMAIL : LoginMethod.ID;
    this._storageService.setLocalStorage(this.localStorageKey, this.loginMethod);
  }
}
