import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  readonly appMaxWidth = 450; // in px  

  private isLargeScreenSubject = new BehaviorSubject<boolean>(false);
  public readonly onLargeScreenChange = this.isLargeScreenSubject.asObservable();

  private loadingStatusSubject = new BehaviorSubject<boolean>(false);
  public readonly onLoadingStatusChange = this.loadingStatusSubject.asObservable();

  setLargeScreen(value: boolean) {
    this.isLargeScreenSubject.next(value);
  }

  isLargeScreen(): boolean {
    return this.isLargeScreenSubject.value;
  }

  setLoadingStatus(value: boolean) {
    setTimeout(() => {
      this.loadingStatusSubject.next(value);
    }, 100);
  }
}
