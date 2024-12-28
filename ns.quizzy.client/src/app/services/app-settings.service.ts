import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  readonly appMaxWidth = 420; // in px  

  private isLargeScreenSubject = new BehaviorSubject<boolean>(false);
  public readonly onLargeScreenChange = this.isLargeScreenSubject.asObservable();

  private loadingStatusSubject = new BehaviorSubject<boolean>(false);
  public readonly onLoadingStatusChange = this.loadingStatusSubject.asObservable();

  constructor(
    private readonly storageService: StorageService,
  ) { }

  setLargeScreen(value: boolean) {
    this.isLargeScreenSubject.next(value);
  }

  isLargeScreen(): boolean {
    return this.isLargeScreenSubject.value;
  }

  setLoadingStatus(value: boolean) {
    this.loadingStatusSubject.next(value);
  }
}
