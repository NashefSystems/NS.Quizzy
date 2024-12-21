import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class LoadingStatusService {
  private loadingStatusSubject = new BehaviorSubject<boolean>(false);
  private loadingStatusChanges$ = this.loadingStatusSubject.asObservable();

  getLoadingStatus() {
    return this.loadingStatusChanges$;
  }

  setLoadingStatus(value: boolean) {
    this.loadingStatusSubject.next(value);
  }
}
