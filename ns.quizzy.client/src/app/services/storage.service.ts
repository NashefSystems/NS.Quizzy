import { Injectable } from '@angular/core';
import { SecurityUtils } from '../utils/security.utils';
import { LocalStorageKeys } from '../enums/local-storage-keys.enum';


@Injectable({
  providedIn: 'root'
})
export class StorageService {
  constructor() { }

  removeLocalStorage(key: LocalStorageKeys) {
    localStorage.removeItem(key);
  }

  setLocalStorage<T>(key: LocalStorageKeys, value: T) {
    var json = value ? JSON.stringify(value) : '';
    localStorage.setItem(key, json);
  }

  getLocalStorage<T>(key: LocalStorageKeys, defaultValue: T | null = null) {
    const json = localStorage.getItem(key) || '';
    if (json) {
      return JSON.parse(json);
    }
    return defaultValue;
  }

  setSensitiveLocalStorage<T>(key: LocalStorageKeys, value: T) {
    localStorage.setItem(key, SecurityUtils.encrypt(value));
  }

  getSensitiveLocalStorage<T>(key: LocalStorageKeys, defaultValue: T | null = null) {
    const encryptedValue = localStorage.getItem(key) || '';
    if (encryptedValue) {
      return SecurityUtils.decrypt(encryptedValue);
    }
    return null;
  }
}
