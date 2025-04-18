import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AppSettingsService {
  readonly homeUrl = '/exam-schedule';
  readonly appMaxWidth = 450; // in px  

  private isLargeScreenModeSubject = new BehaviorSubject<boolean>(false);
  public readonly onLargeScreenModeChange = this.isLargeScreenModeSubject.asObservable();

  private loadingStatusSubject = new BehaviorSubject<boolean>(false);
  public readonly onLoadingStatusChange = this.loadingStatusSubject.asObservable();

  private appMaxWidthSubject = new BehaviorSubject<number>(window.innerWidth);
  public readonly onAppMaxWidthChange = this.appMaxWidthSubject.asObservable();

  private appMaxHeightSubject = new BehaviorSubject<number>(window.innerHeight);
  public readonly onAppMaxHeightChange = this.appMaxHeightSubject.asObservable();

  setLargeScreenMode(value: boolean) {
    this.isLargeScreenModeSubject.next(value);
  }

  isLargeScreenMode(): boolean {
    return this.isLargeScreenModeSubject.value;
  }

  setAppMaxWidth(value: number) {
    this.appMaxWidthSubject.next(value);
  }

  getAppMaxWidth(): number {
    return this.appMaxWidthSubject.value;
  }

  setAppMaxHeight(value: number) {
    this.appMaxHeightSubject.next(value);
  }

  getAppMaxHeight(): number {
    return this.appMaxHeightSubject.value;
  }

  setLoadingStatus(value: boolean) {
    setTimeout(() => {
      this.loadingStatusSubject.next(value);
    }, 100);
  }
}
