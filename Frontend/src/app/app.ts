import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { APP_CONSTANTS } from './core/constants/app.constants';
import { GlobalLoader } from './core/components/global-loader/global-loader';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, GlobalLoader],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = APP_CONSTANTS.STRINGS.APP_NAME;
}