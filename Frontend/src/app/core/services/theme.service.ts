import { Injectable, signal } from '@angular/core';
import { APP_CONSTANTS, ThemeMode } from '../constants/app.constants';

export interface ThemePreference {
  mode: ThemeMode;
  color: string;
  font: string;
}

const THEME = APP_CONSTANTS.THEME;

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly mediaQuery = window.matchMedia?.('(prefers-color-scheme: dark)');

  readonly preference = signal<ThemePreference>(this.loadPreference());

  constructor() {
    this.applyPreference(this.preference());
    this.mediaQuery?.addEventListener?.('change', () => {
      if (this.preference().mode === 'system') {
        this.applyPreference(this.preference());
      }
    });
  }

  private loadPreference(): ThemePreference {
    try {
      const raw = localStorage.getItem(THEME.STORAGE_KEY);
      if (raw) {
        const parsed = JSON.parse(raw) as Partial<ThemePreference>;
        return {
          mode: parsed.mode ?? THEME.DEFAULT_MODE,
          color: parsed.color ?? THEME.DEFAULT_COLOR,
          font: parsed.font ?? THEME.DEFAULT_FONT,
        };
      }
    } catch {
      /* ignore malformed storage */
    }
    return { mode: THEME.DEFAULT_MODE, color: THEME.DEFAULT_COLOR, font: THEME.DEFAULT_FONT };
  }

  setMode(mode: ThemeMode): void {
    this.update({ mode });
  }

  setColor(colorKey: string): void {
    this.update({ color: colorKey });
  }

  setFont(fontKey: string): void {
    this.update({ font: fontKey });
  }

  private update(partial: Partial<ThemePreference>): void {
    const next = { ...this.preference(), ...partial };
    this.preference.set(next);
    localStorage.setItem(THEME.STORAGE_KEY, JSON.stringify(next));
    this.applyPreference(next);
  }

  private resolvedMode(mode: ThemeMode): 'light' | 'dark' {
    if (mode === 'system') {
      return this.mediaQuery?.matches ? 'dark' : 'light';
    }
    return mode;
  }

  private applyPreference(pref: ThemePreference): void {
    const root = document.documentElement;
    root.setAttribute('data-theme', this.resolvedMode(pref.mode));

    const colorOption = THEME.COLOR_OPTIONS.find((c) => c.key === pref.color) ?? THEME.COLOR_OPTIONS[0];
    const fontOption = THEME.FONT_OPTIONS.find((f) => f.key === pref.font) ?? THEME.FONT_OPTIONS[0];

    root.style.setProperty('--color-primary', colorOption.value);
    root.style.setProperty('--color-primary-dark', colorOption.dark);
    root.style.setProperty('--color-primary-light', `${colorOption.value}1a`);
    root.style.setProperty('--font-family-base', fontOption.value);
  }
}
