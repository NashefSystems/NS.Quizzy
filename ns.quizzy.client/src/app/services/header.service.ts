import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HeaderService {
  private headerTitleSubject = new BehaviorSubject<string>("");
  private headerTitleChanges$ = this.headerTitleSubject.asObservable();

  getHeaderTitle() {
    return this.headerTitleChanges$;
  }

  setHeaderTitle(value: string) {
    this.headerTitleSubject.next(value);
  }
}
