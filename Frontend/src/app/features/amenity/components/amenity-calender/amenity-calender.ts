import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { AMENITY_CALENDER_STRINGS, WEEK_DAYS, CALENDER_NUMBERS } from '../../../../core/constants/amenity.constants';
import { IsPastPipe } from '../../pipes/is-past.pipe';
import { IsTodayPipe } from '../../pipes/is-today.pipe';
import { IsSelectedDayPipe } from '../../pipes/is-selected-day.pipe';

@Component({
  selector: 'app-amenity-calender',
  standalone: true,
  imports: [CommonModule, IsPastPipe, IsTodayPipe, IsSelectedDayPipe, MatButtonModule],
  templateUrl: './amenity-calender.html',
  styleUrl: './amenity-calender.scss',
})
export class AmenityCalender implements OnInit {
  @Input() selectedDate: Date = new Date();
  @Output() dateSelected = new EventEmitter<Date>();

  calenderStrings = AMENITY_CALENDER_STRINGS;
  currentMonth: Date = new Date();
  daysInMonth: number[] = [];
  blankDays: number[] = [];
  weekDays = WEEK_DAYS;

  ngOnInit(): void {
    this.currentMonth = new Date(
      this.selectedDate.getFullYear(),
      this.selectedDate.getMonth(),
      CALENDER_NUMBERS.FIRST_DAY
    );
    this.generateCalendar();
  }

  generateCalendar(): void {
    const year = this.currentMonth.getFullYear();
    const month = this.currentMonth.getMonth();
    
    const firstDay = new Date(year, month, CALENDER_NUMBERS.FIRST_DAY);
    const dayOfWeek = firstDay.getDay();
    const firstDayIndex = dayOfWeek === CALENDER_NUMBERS.ZERO
      ? CALENDER_NUMBERS.SUNDAY_INDEX
      : dayOfWeek - CALENDER_NUMBERS.FIRST_DAY;

    const totalDays = new Date(year, month + CALENDER_NUMBERS.FIRST_DAY, CALENDER_NUMBERS.ZERO).getDate();

    this.blankDays = Array(firstDayIndex).fill(CALENDER_NUMBERS.ZERO);
    this.daysInMonth = Array.from({ length: totalDays }, (_, i) => i + CALENDER_NUMBERS.FIRST_DAY);
  }

  prevMonth(): void {
    if (!this.canGoPrev()) return;
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + CALENDER_NUMBERS.MONTH_OFFSET_PREV,
      CALENDER_NUMBERS.FIRST_DAY
    );
    this.generateCalendar();
  }

  nextMonth(): void {
    this.currentMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + CALENDER_NUMBERS.MONTH_OFFSET_NEXT,
      CALENDER_NUMBERS.FIRST_DAY
    );
    this.generateCalendar();
  }

  canGoPrev(): boolean {
    const today = new Date();
    const currentMonthFirstDay = new Date(today.getFullYear(), today.getMonth(), CALENDER_NUMBERS.FIRST_DAY);
    const targetMonth = new Date(
      this.currentMonth.getFullYear(),
      this.currentMonth.getMonth() + CALENDER_NUMBERS.MONTH_OFFSET_PREV,
      CALENDER_NUMBERS.FIRST_DAY
    );
    return targetMonth >= currentMonthFirstDay;
  }

  selectDate(day: number): void {
    if (this.isPast(day)) return;
    const newDate = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), day);
    this.selectedDate = newDate;
    this.dateSelected.emit(newDate);
  }

  isPast(day: number): boolean {
    const d = new Date(this.currentMonth.getFullYear(), this.currentMonth.getMonth(), day);
    const today = new Date();
    today.setHours(
      CALENDER_NUMBERS.HOURS_MIDNIGHT,
      CALENDER_NUMBERS.MINUTES_MIDNIGHT,
      CALENDER_NUMBERS.SECONDS_MIDNIGHT,
      CALENDER_NUMBERS.MILLISECONDS_MIDNIGHT
    );
    d.setHours(
      CALENDER_NUMBERS.HOURS_MIDNIGHT,
      CALENDER_NUMBERS.MINUTES_MIDNIGHT,
      CALENDER_NUMBERS.SECONDS_MIDNIGHT,
      CALENDER_NUMBERS.MILLISECONDS_MIDNIGHT
    );
    return d < today;
  }
}
