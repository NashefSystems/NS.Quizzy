import { Component, inject, Input } from '@angular/core';
import { Location } from '@angular/common';
import { AppTranslateService } from '../../../services/app-translate.service';
import { Directions } from '../../../enums/directions.enum';

@Component({
  selector: 'app-back-button',
  standalone: false,
  templateUrl: './back-button.component.html',
  styleUrl: './back-button.component.scss'
})
export class BackButtonComponent {
  private readonly _location = inject(Location);
  private readonly _appTranslateService = inject(AppTranslateService);
  buttonIcon: string = '';

  @Input()
  marginBottom: string = '2rem';

  constructor() {
    this._appTranslateService.onDirectionChange.subscribe({
      next: (dir) => {
        this.buttonIcon = dir === Directions.LTR ? 'arrow_back' : 'arrow_forward';
      }
    });
  }

  goBack() {
    this._location.back();
  }
}
