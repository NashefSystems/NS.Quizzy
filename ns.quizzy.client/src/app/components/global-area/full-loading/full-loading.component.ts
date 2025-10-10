import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-full-loading',
  standalone: false,

  templateUrl: './full-loading.component.html',
  styleUrl: './full-loading.component.scss'
})
export class FullLoadingComponent {
  @Input() message: string = 'FULL_LOADING.MESSAGE';
  @Input() size: 'small' | 'medium' | 'large' = 'small';
}
