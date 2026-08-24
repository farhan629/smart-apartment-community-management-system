import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

import { PERMISSIONS, Permission } from '../../../../core/constants/permission.constants';
import { APP_CONSTANTS } from '../../../../core/constants/app.constants';

@Component({
  selector: 'app-permission-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './permission-list.html',
  styleUrl: './permission-list.scss',
})
export class PermissionListComponent {
  readonly permissionsList: Permission[] = Object.values(PERMISSIONS);
  readonly strings = APP_CONSTANTS.STRINGS;

  @Input() permissionState: Record<string, boolean> = {};
  @Input() loading = false;
  @Input() saving = false;
  @Input() error = '';
  @Input() success = '';
  @Input() hasChanges = false;

  @Output() toggle = new EventEmitter<string>();
  @Output() update = new EventEmitter<void>();
}