import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-popup-dialog',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './popup-dialog.html',
  styleUrl: './popup-dialog.scss',
})
export class PopupDialog {
  @Input({ required: true }) title = '';
  @Input() width = 'auto';
  @Output() closed = new EventEmitter<void>();

  icons = APP_CONSTANTS.ICONS;

  onClose(): void {
    this.closed.emit();
  }
}
