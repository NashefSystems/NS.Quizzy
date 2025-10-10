import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'camelToSnake',
  standalone: false,
})
export class CamelToSnakePipe implements PipeTransform {
  transform(value: string): string {
    return value.replace(/[A-Z]/g, letter => `_${letter}`).toUpperCase().replace(/^_/, '');
  }
}
