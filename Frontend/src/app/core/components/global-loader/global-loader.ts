import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoaderService } from '../../services/loader.service';

@Component({
  selector: 'app-global-loader',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './global-loader.html',
  styleUrl: './global-loader.scss',
})
export class GlobalLoader {
  readonly loaderService = inject(LoaderService);
}
