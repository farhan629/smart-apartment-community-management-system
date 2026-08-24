import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'formatTime',
  standalone: true
})
export class FormatTimePipe implements PipeTransform {
  transform(time: string | undefined | null): string {
    if (!time) return '';
    
    const parts = time.split(':');
    if (parts.length < 2) return time;

    let hours = parseInt(parts[0], 10);
    const minutes = parts[1];

    if (isNaN(hours)) return time;

    const ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; 
    
    const hoursStr = String(hours).padStart(2, '0');

    return `${hoursStr}:${minutes} ${ampm}`;
  }
}
