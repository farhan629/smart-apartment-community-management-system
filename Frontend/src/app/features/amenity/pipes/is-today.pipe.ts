import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'isToday',
  standalone: true
})
export class IsTodayPipe implements PipeTransform {
  transform(day: number, currentMonth: Date): boolean {
    const d = new Date(currentMonth.getFullYear(), currentMonth.getMonth(), day);
    const today = new Date();
    return d.toDateString() === today.toDateString();
  }
}
