import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.scss',
})
export class StatusBadge {
  @Input({ required: true }) status = '';

  get colorKey(): string {
    return APP_CONSTANTS.STATUS_COLOR_KEY[this.status] ?? 'neutral';
  }
}