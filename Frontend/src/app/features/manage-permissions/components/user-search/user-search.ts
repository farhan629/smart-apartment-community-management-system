import { Component, EventEmitter, Output, Input, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatIconModule } from '@angular/material/icon';
import { Subject, debounceTime, distinctUntilChanged, switchMap, tap, catchError, of } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { UserSearchResultDto } from '../../../../core/models/manage-permissions.models';
import { ManagePermissionsService } from '../../services/manage-permissions.service';

@Component({
  selector: 'app-user-search',
  standalone: true,
  imports: [CommonModule, FormsModule, MatIconModule],
  templateUrl: './user-search.html',
  styleUrl: './user-search.scss',
})
export class UserSearchComponent {
  private readonly service = inject(ManagePermissionsService);

  readonly icons = APP_CONSTANTS.ICONS;
  readonly strings = APP_CONSTANTS.STRINGS;
  readonly searchResults = signal<UserSearchResultDto[]>([]);
  readonly searching = signal(false);

  @Input() selectedUser: UserSearchResultDto | null = null;
  @Output() userSelected = new EventEmitter<UserSearchResultDto>();
  @Output() cleared = new EventEmitter<void>();

  searchTerm = '';
  private readonly searchSubject = new Subject<string>();

  constructor() {
    this.searchSubject
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        tap(() => this.searching.set(true)),
        switchMap((term) =>
          term.trim()
            ? this.service.searchUsers(term).pipe(catchError(() => of([])))
            : of([]),
        ),
        tap((results) => {
          this.searchResults.set(results);
          this.searching.set(false);
        }),
        takeUntilDestroyed(),
      )
      .subscribe();
  }

  onSearchInput(term: string): void {
    this.searchTerm = term;
    this.searchSubject.next(term);
  }

  selectUser(user: UserSearchResultDto): void {
    this.userSelected.emit(user);
    this.searchResults.set([]);
  }

  clearSelection(): void {
    this.searchTerm = '';
    this.searchResults.set([]);
    this.cleared.emit();
  }
}