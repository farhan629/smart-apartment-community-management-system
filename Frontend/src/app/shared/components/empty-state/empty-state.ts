import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';
import { APP_CONSTANTS } from '../../../core/constants/app.constants';
@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './empty-state.html',
  styleUrl: './empty-state.scss',
})
export class EmptyState {
  @Input() icon: string = APP_CONSTANTS.ICONS.INBOX;
  @Input() message: string = APP_CONSTANTS.STRINGS.NO_DATA;
  @Input() subMessage = '';
}
