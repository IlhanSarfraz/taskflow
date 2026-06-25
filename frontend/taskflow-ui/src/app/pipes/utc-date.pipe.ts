import { Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
  name: 'utcDate',
  standalone: true
})
export class UtcDatePipe implements PipeTransform {
  private readonly datePipe = new DatePipe('en-US');

  transform(value: string, format: string = 'MMM d, y'): string {
    if (!value) return '';
    const utc = value.endsWith('Z') ? value : value + 'Z';
    return this.datePipe.transform(utc, format) ?? '';
  }
}