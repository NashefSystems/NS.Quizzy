import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-exam-info-value',
  standalone: false,

  templateUrl: './exam-info-value.component.html',
  styleUrl: './exam-info-value.component.scss'
})
export class ExamInfoValueComponent {
  @Input() iconColor: string;
  @Input() icon: string;
  @Input() name: string;
  @Input() value: string | null | undefined | number;
}
