import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'time',
  standalone: false,
})
export class TimePipe implements PipeTransform {
  // Value format: HH:mm:ss
  transform(value: any, format: 'HH:mm' = 'HH:mm'): string {
    if (!value) {
      return '';
    }

    const [h, m, _] = value.split(':');
    return `${h}:${m}`;
  }
}
