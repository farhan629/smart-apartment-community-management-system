import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'isSelectedDay',
  standalone: true
})
export class IsSelectedDayPipe implements PipeTransform {
  transform(day: number, currentMonth: Date, selectedDate: Date): boolean {
    const d = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), day);
    return d.toDateString() === selectedDate.toDateString();
  }
}
