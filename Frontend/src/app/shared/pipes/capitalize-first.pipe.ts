import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'capitalizeFirst',
  standalone: true,
})
export class CapitalizeFirstPipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value) {
      return '';
    }

    const trimmed = value.trimStart();
    if (!trimmed) {
      return value;
    }

    return trimmed.charAt(0).toUpperCase() + trimmed.slice(1);
  }
}
