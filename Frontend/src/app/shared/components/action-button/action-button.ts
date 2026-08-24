import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output, ElementRef, inject } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

export type ActionButtonVariant = 'primary' | 'secondary' | 'danger';

export interface ActionMenuItem {
  label: string;
  icon: string;
  action: string;
}

@Component({
  selector: 'app-action-button',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './action-button.html',
  styleUrl: './action-button.scss',
})
export class ActionButton {
  private readonly el = inject(ElementRef);

  @Input() label = '';
  @Input() icon = '';
  @Input() variant: ActionButtonVariant = 'primary';
  @Input() disabled = false;
  @Input() type: 'button' | 'submit' = 'button';
  @Input() showDropdown = false;
  @Input() menuItems: ActionMenuItem[] = [];

  @Output() clicked = new EventEmitter<void>();
  @Output() menuAction = new EventEmitter<string>();

  isMenuOpen = false;
  menuOpenUp = false;
  menuStyle: Record<string, string> = {};

  onClick(): void {
    if (this.disabled) { return; }
    if (this.showDropdown) {
      this.isMenuOpen = !this.isMenuOpen;
      if (this.isMenuOpen) {
        const btn = this.el.nativeElement.querySelector('.action-btn') as HTMLElement;
        if (btn) {
          const rect = btn.getBoundingClientRect();
          const gap = 6;
          const estimatedMenuHeight = 8 + this.menuItems.length * 40;
          const spaceBelow = window.innerHeight - rect.bottom;
          const spaceAbove = rect.top;

          if (spaceBelow < estimatedMenuHeight + gap && spaceAbove > estimatedMenuHeight + gap) {
            this.menuOpenUp = true;
            this.menuStyle = {
              position: 'fixed',
              top: `${rect.top - gap - estimatedMenuHeight}px`,
              right: `${window.innerWidth - rect.right}px`,
              zIndex: '1000',
            };
          } else {
            this.menuOpenUp = false;
            this.menuStyle = {
              position: 'fixed',
              top: `${rect.bottom + gap}px`,
              right: `${window.innerWidth - rect.right}px`,
              zIndex: '1000',
            };
          }
        }
      }
    } else {
      this.clicked.emit();
    }
  }

  onMenuItemClick(action: string): void {
    this.isMenuOpen = false;
    this.menuAction.emit(action);
  }

  onBackdropClick(): void {
    this.isMenuOpen = false;
  }
}
