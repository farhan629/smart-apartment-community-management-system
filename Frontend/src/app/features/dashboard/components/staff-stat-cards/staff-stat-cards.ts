import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatIconModule } from '@angular/material/icon';

export interface StaffStatCard {
  key: string;
  label: string;
  icon: string;
  accent: 'success' | 'info' | 'danger' | 'warning' | 'primary';
  value: number;
}

@Component({
  selector: 'app-staff-stat-cards',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './staff-stat-cards.html',
  styleUrl: './staff-stat-cards.scss',
})
export class StaffStatCards {
  @Input({ required: true }) cards: StaffStatCard[] = [];
}
