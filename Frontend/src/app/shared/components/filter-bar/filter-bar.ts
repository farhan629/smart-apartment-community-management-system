import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';

export interface FilterOption {
  label: string;
  value: string;
}

export interface FilterDropdownConfig {
  key: string;
  placeholder: string;
  options: FilterOption[];
}

export type FilterValues = Record<string, string>;

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './filter-bar.html',
  styleUrl: './filter-bar.scss',
})
export class FilterBar {
  @Input() dropdowns: FilterDropdownConfig[] = [];
  @Output() filterChange = new EventEmitter<FilterValues>();

  strings = APP_CONSTANTS.STRINGS;
  selectedValues: FilterValues = {};

  onDropdownChange(key: string, value: string): void {
    if (value) {
      this.selectedValues[key] = value;
    } else {
      delete this.selectedValues[key];
    }
    this.filterChange.emit({ ...this.selectedValues });
  }

  onReset(): void {
    this.selectedValues = {};
    this.filterChange.emit({});
  }
}
