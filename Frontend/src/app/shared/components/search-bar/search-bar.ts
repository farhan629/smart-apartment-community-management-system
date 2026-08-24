import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-search-bar',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './search-bar.html',
  styleUrl: './search-bar.scss',
})
export class SearchBar {
  @Input() placeholder: string = APP_CONSTANTS.STRINGS.SEARCH_PLACEHOLDER;
  @Output() searchChange = new EventEmitter<string>();

  icons = APP_CONSTANTS.ICONS;
  term = '';
  private debounceTimer?: ReturnType<typeof setTimeout>;

  onInput(): void {
    clearTimeout(this.debounceTimer);
    this.debounceTimer = setTimeout(() => this.searchChange.emit(this.term), 300);
  }
}
