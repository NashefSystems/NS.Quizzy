import { Injectable } from '@angular/core';
import { LangChangeEvent, TranslateService } from '@ngx-translate/core';
import { AppLanguages } from '../enums/app-languages.enum';
import { Directions } from '../enums/directions.enum';
import { BehaviorSubject } from 'rxjs';
import { StorageService } from './storage.service';
import { LocalStorageKeys } from '../enums/local-storage-keys.enum';

@Injectable({
  providedIn: 'root'
})
export class AppTranslateService {
  private DirectionSubject = new BehaviorSubject<Directions>(Directions.RTL);
  public readonly onDirectionChange = this.DirectionSubject.asObservable();

  constructor(
    private readonly translateService: TranslateService,
    private readonly storageService: StorageService,
  ) {
    this.init();
  }

  private init() {
    this.translateService.setDefaultLang('he');
    const appLang = this.storageService.getLocalStorage(LocalStorageKeys.appLaguage, AppLanguages.HE);
    this.translateService.use(appLang);

    this.translateService.onLangChange.subscribe((event: LangChangeEvent) => {
      const direction = event.translations['DIR'];
      this.DirectionSubject.next(direction == 'ltr' ? Directions.LTR : Directions.RTL);
    });

    // Set initial direction
    const initialDir = this.translate('DIR');
    this.DirectionSubject.next(initialDir == 'ltr' ? Directions.LTR : Directions.RTL);
  }

  translate(key: string, params: { [key: string]: any; } = {}): string {
    return this.translateService.instant(key, params);
  }

  switchLanguage(language: AppLanguages): void {
    this.translateService.use(language);
    this.storageService.setLocalStorage(LocalStorageKeys.appLaguage, language);
  }

  isRtlDirection() {
    return this.DirectionSubject.value === Directions.RTL;
  }
}