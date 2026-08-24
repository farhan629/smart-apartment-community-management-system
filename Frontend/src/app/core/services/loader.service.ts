import { Injectable, signal } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LoaderService {
  private readonly requestCount = signal(0);
  readonly isVisible = signal(false);

  show(): void {
    this.requestCount.update((count) => count + 1);
    this.isVisible.set(true);
  }

  hide(): void {
    this.requestCount.update((count) => Math.max(0, count - 1));
    if (this.requestCount() === 0) {
      this.isVisible.set(false);
    }
  }
}
