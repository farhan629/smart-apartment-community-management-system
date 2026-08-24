import { Pipe, PipeTransform } from '@angular/core';
import { CALENDER_NUMBERS } from '../../../core/constants/amenity.constants';

@Pipe({
  name: 'isPast',
  standalone: true
})
export class IsPastPipe implements PipeTransform {
  transform(day: number, currentMonth: Date): boolean {
    const d = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), day);
    const today = new Date();
    today.setHours(
      CALENDER_NUMBERS.HOURS_MIDNIGHT,
      CALENDER_NUMBERS.MINUTES_MIDNIGHT,
      CALENDER_NUMBERS.SECONDS_MIDNIGHT,
      CALENDER_NUMBERS.MILLISECONDS_MIDNIGHT
    );
    d.setHours(
      CALENDER_NUMBERS.HOURS_MIDNIGHT,
      CALENDER_NUMBERS.MINUTES_MIDNIGHT,
      CALENDER_NUMBERS.SECONDS_MIDNIGHT,
      CALENDER_NUMBERS.MILLISECONDS_MIDNIGHT
    );
    return d < today;
  }
}
