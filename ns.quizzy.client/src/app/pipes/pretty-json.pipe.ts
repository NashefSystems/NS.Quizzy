import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'prettyJson',
  standalone: false,
})
export class PrettyJsonPipe implements PipeTransform {
  transform(value: any): string {
    return JSON.stringify(value, null, 10/*'\t'*/);
  }
}
