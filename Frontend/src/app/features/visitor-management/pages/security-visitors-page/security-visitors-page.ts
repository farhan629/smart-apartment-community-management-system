import { CommonModule } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { forkJoin } from 'rxjs';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import {
  HISTORY_VISIT_STATUSES,
  UPCOMING_VISIT_STATUSES,
  VISIT_STATUS_LABELS,
  VISIT_TABS,
  VisitTab,
} from '../../../../core/constants/visit.constants';
import {
  VISITOR_MANAGEMENT_ICONS,
  VISITOR_MANAGEMENT_STRINGS,
  VISIT_LIST_MERGE_FETCH_LIMIT,
} from '../../../../core/constants/visitor-management-ui.constants';
import { FlatItemDto } from '../../../../core/models/auth.models';
import { Visit } from '../../../../core/models/visit.model';
import { FlatService } from '../../../../core/services/flat-service';
import { VisitService } from '../../../../core/services/visit.service';
import {
  canApproveVisit,
  canCancelVisit,
  canUpdateVisit,
} from '../../../../core/utils/visit-actions.util';
import { ActionButton } from '../../../../shared/components/action-button/action-button';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { RejectVisitDialog } from '../../../../shared/components/reject-visit-dialog/reject-visit-dialog';
import { StatusBadge } from '../../../../shared/components/status-badge/status-badge';
import { UpdateVisitorDialog } from '../../components/update-visitor-dialog/update-visitor-dialog';
import { VisitDetailDialog } from '../../components/visit-detail-dialog/visit-detail-dialog';

type ViewLevel = 'blocks' | 'flats' | 'visits';

interface BlockSummary {
  name: string;
  flatCount: number;
}

@Component({
  selector: 'app-security-visitors-page',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatProgressSpinnerModule, ActionButton, StatusBadge],
  templateUrl: './security-visitors-page.html',
  styleUrl: './security-visitors-page.scss',
})
export class SecurityVisitorsPage implements OnInit {
  private readonly flatService = inject(FlatService);
  private readonly visitService = inject(VisitService);
  private readonly dialog = inject(MatDialog);

  strings = APP_CONSTANTS.STRINGS;
  vm = VISITOR_MANAGEMENT_STRINGS;
  icons = VISITOR_MANAGEMENT_ICONS;
  tabs = VISIT_TABS;

  level = signal<ViewLevel>('blocks');

  allFlats = signal<FlatItemDto[]>([]);
  blocks = signal<BlockSummary[]>([]);

  selectedBlock = signal('');
  selectedFlat = signal<FlatItemDto | null>(null);

  activeTab = signal<VisitTab>(VISIT_TABS.UPCOMING);
  visits = signal<Visit[]>([]);

  isLoadingFlats = signal(false);
  isLoadingVisits = signal(false);
  errorMessage = signal('');

  readonly flatsInSelectedBlock = computed(() =>
    this.allFlats()
      .filter((f) => f.block === this.selectedBlock())
      .sort((a, b) => a.number.localeCompare(b.number, undefined, { numeric: true })),
  );

  ngOnInit(): void {
    this.loadFlats();
  }

  private loadFlats(): void {
    this.isLoadingFlats.set(true);
    this.errorMessage.set('');

    this.flatService.getFlats().subscribe({
      next: (response) => {
        this.allFlats.set(response.items);
        this.blocks.set(this.groupByBlock(response.items));
        this.isLoadingFlats.set(false);
      },
      error: () => {
        this.errorMessage.set(this.vm.BLOCKS_LOAD_FAILED);
        this.isLoadingFlats.set(false);
      },
    });
  }

  private groupByBlock(flats: FlatItemDto[]): BlockSummary[] {
    const counts = new Map<string, number>();
    for (const flat of flats) {
      counts.set(flat.block, (counts.get(flat.block) ?? 0) + 1);
    }
    return [...counts.entries()]
      .map(([name, flatCount]) => ({ name, flatCount }))
      .sort((a, b) => a.name.localeCompare(b.name));
  }

  onSelectBlock(block: BlockSummary): void {
    this.selectedBlock.set(block.name);
    this.level.set('flats');
  }

  onSelectFlat(flat: FlatItemDto): void {
    this.selectedFlat.set(flat);
    this.activeTab.set(VISIT_TABS.UPCOMING);
    this.level.set('visits');
    this.loadVisitsForFlat();
  }

  onBackToBlocks(): void {
    this.level.set('blocks');
    this.selectedBlock.set('');
    this.selectedFlat.set(null);
  }

  onBackToFlats(): void {
    this.level.set('flats');
    this.selectedFlat.set(null);
    this.visits.set([]);
  }

  onTabChange(tab: VisitTab): void {
    this.activeTab.set(tab);
    this.loadVisitsForFlat();
  }

  loadVisitsForFlat(): void {
    const selectedFlat = this.selectedFlat();
    if (!selectedFlat) return;

    const statuses =
      this.activeTab() === this.tabs.UPCOMING ? UPCOMING_VISIT_STATUSES : HISTORY_VISIT_STATUSES;
    const sortOrder = this.activeTab() === this.tabs.UPCOMING ? 'asc' : 'desc';

    this.isLoadingVisits.set(true);
    this.errorMessage.set('');

    forkJoin(
      statuses.map((status) =>
        this.visitService.getVisits({
          flatId: selectedFlat.id,
          status,
          sortBy: 'startDate',
          sortOrder,
          page: 1,
          limit: VISIT_LIST_MERGE_FETCH_LIMIT,
        }),
      ),
    ).subscribe({
      next: (responses) => {
        const items = responses.flatMap((r) => r?.items ?? []);
        items.sort((a, b) => {
          const diff = new Date(a.startDate).getTime() - new Date(b.startDate).getTime();
          return sortOrder === 'asc' ? diff : -diff;
        });
        this.visits.set(items);
        this.isLoadingVisits.set(false);
      },
      error: () => {
        this.visits.set([]);
        this.errorMessage.set(this.strings.VISITS_LOAD_FAILED);
        this.isLoadingVisits.set(false);
      },
    });
  }

  getStatusLabel(status: string): string {
    return (VISIT_STATUS_LABELS as Record<string, string>)[status] ?? status;
  }

  canUpdate(visit: Visit): boolean {
    return canUpdateVisit(visit.status);
  }

  canApprove(visit: Visit): boolean {
    return canApproveVisit(visit.status);
  }

  canCancel(visit: Visit): boolean {
    return canCancelVisit(visit.status);
  }

  onViewDetail(visit: Visit): void {
    const dialogRef = this.dialog.open(VisitDetailDialog, {
      width: '32rem',
      data: { visitId: visit.id },
    });

    dialogRef.afterClosed().subscribe((changed) => {
      if (changed) this.loadVisitsForFlat();
    });
  }

  onUpdateVisitor(visit: Visit): void {
    const dialogRef = this.dialog.open(UpdateVisitorDialog, {
      width: '32rem',
      data: { visitorId: visit.visitorId },
    });

    dialogRef.afterClosed().subscribe((updated) => {
      if (updated) this.loadVisitsForFlat();
    });
  }

  onCancelVisit(visit: Visit): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.strings.CANCEL_VISIT_TITLE,
        message: this.strings.CANCEL_VISIT_MESSAGE,
        variant: 'danger',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.visitService.cancelVisit(visit.id).subscribe({
          next: () => this.loadVisitsForFlat(),
          error: () => {
            this.errorMessage.set(this.strings.VISIT_CANCEL_FAILED);
          },
        });
      }
    });
  }

  onApproveVisit(visit: Visit): void {
    const dialogRef = this.dialog.open(ConfirmDialog, {
      data: {
        title: this.vm.APPROVE_VISIT_TITLE,
        message: this.vm.APPROVE_VISIT_MESSAGE,
        variant: 'primary',
      },
    });

    dialogRef.afterClosed().subscribe((confirmed) => {
      if (confirmed) {
        this.visitService.approveVisit(visit.id).subscribe({
          next: () => this.loadVisitsForFlat(),
          error: () => {
            this.errorMessage.set(this.strings.VISIT_APPROVE_FAILED);
          },
        });
      }
    });
  }

  onRejectVisit(visit: Visit): void {
    const dialogRef = this.dialog.open(RejectVisitDialog, { width: '30rem' });

    dialogRef.afterClosed().subscribe((rejectionReason) => {
      if (rejectionReason) {
        this.visitService.rejectVisit(visit.id, { rejectionReason }).subscribe({
          next: () => this.loadVisitsForFlat(),
          error: () => {
            this.errorMessage.set(this.strings.VISIT_REJECT_FAILED);
          },
        });
      }
    });
  }
}
