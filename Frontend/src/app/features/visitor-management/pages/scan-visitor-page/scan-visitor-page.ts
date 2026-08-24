import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, ViewChild, inject, signal } from '@angular/core';
import { NgxScannerQrcodeComponent, ScannerQRCodeResult } from 'ngx-scanner-qrcode';

import { APP_CONSTANTS } from '../../../../core/constants/app.constants';
import { SCAN_DIRECTION, ScanDirection } from '../../../../core/constants/visit.constants';
import { VisitService } from '../../../../core/services/visit.service';

@Component({
  selector: 'app-scan-visitor-page',
  standalone: true,
  imports: [CommonModule, NgxScannerQrcodeComponent],
  templateUrl: './scan-visitor-page.html',
  styleUrl: './scan-visitor-page.scss',
})
export class ScanVisitorPage implements AfterViewInit {
  private readonly visitService = inject(VisitService);

  @ViewChild('scanner') scanner!: NgxScannerQrcodeComponent;

  strings = APP_CONSTANTS.STRINGS;
  directions = SCAN_DIRECTION;

  direction = signal<ScanDirection>(SCAN_DIRECTION.CHECK_IN);
  isProcessing = signal(false);
  resultMessage = signal('');
  resultIsError = signal(false);
  cameraError = signal(false);

  ngAfterViewInit(): void {
    this.scanner.start().subscribe({
      error: () => this.cameraError.set(true),
    });
  }

  onDirectionChange(direction: ScanDirection): void {
    this.direction.set(direction);
    this.resultMessage.set('');
  }

  onScanSuccess(results: ScannerQRCodeResult[]): void {
    if (!results?.length || this.isProcessing()) {
      return;
    }

    const token = results[0]?.value;
    if (!token) {
      return;
    }

    this.isProcessing.set(true);
    this.resultMessage.set('');

    const scan$ =
      this.direction() === SCAN_DIRECTION.CHECK_IN
        ? this.visitService.checkInByToken(token)
        : this.visitService.checkOutByToken(token);

    scan$.subscribe({
      next: () => {
        this.resultIsError.set(false);
        this.resultMessage.set(
          this.direction() === SCAN_DIRECTION.CHECK_IN
            ? this.strings.SCAN_CHECKIN_SUCCESS
            : this.strings.SCAN_CHECKOUT_SUCCESS,
        );
        this.isProcessing.set(false);
      },
      error: (err) => {
        this.resultIsError.set(true);
        this.resultMessage.set(err?.error?.message ?? this.strings.SCAN_FAILED);
        this.isProcessing.set(false);
      },
    });
  }
}