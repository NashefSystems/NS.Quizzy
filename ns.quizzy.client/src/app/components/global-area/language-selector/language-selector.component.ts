import { Component } from '@angular/core';
import { AppLanguages } from '../../../enums/app-languages.enum';
import { AppTranslateService } from '../../../services/app-translate.service';

@Component({
  selector: 'app-language-selector',
  standalone: false,
  templateUrl: './language-selector.component.html',
  styleUrl: './language-selector.component.scss'
})
export class LanguageSelectorComponent {
  AppLanguages = AppLanguages;

  constructor(
    private readonly appTranslateService: AppTranslateService
  ) { }

  switchLanguage(language: AppLanguages): void {
    this.appTranslateService.switchLanguage(language);
  }
}
