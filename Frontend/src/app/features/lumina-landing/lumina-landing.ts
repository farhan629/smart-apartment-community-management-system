import {
  AfterViewInit,
  ChangeDetectorRef,
  Component,
  ElementRef,
  HostListener,
  inject,
  OnDestroy,
  OnInit,
  ViewChild,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeHtml } from '@angular/platform-browser';
import gsap from 'gsap';
import { ScrollTrigger } from 'gsap/ScrollTrigger';
import {
  BRAND,
  NAV,
  HERO_CONTENT,
  FEATURES_SECTION,
  DASHBOARD_SECTION,
  TECH_SECTION,
  CTA_SECTION,
  FOOTER_LINKS,
  FEATURES,
  DASH_SIDEBAR_ITEMS,
  DASHBOARD_STATS,
  CHART_BARS,
  TECH_COLUMNS,
  CANVAS_CONFIG,
  SCROLL_ANIMATION_CONFIG,
  FeatureItem,
  TechColumn,
  DashboardStat,
  DashSidebarItem,
  ChartBar,
  FooterLink,
} from '../../core/constants/lumina-landing.constants';
import { routes } from '../../app.routes';
import { Router } from '@angular/router';
import { AuthService } from '../../core/services/auth-service';

gsap.registerPlugin(ScrollTrigger);

interface Particle {
  x: number;
  y: number;
  r: number;
  speed: number;
  phase: number;
}

interface Building {
  x: number;
  w: number;
  h: number;
  delay: number;
  windows: number;
}

@Component({
  selector: 'app-lumina-landing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './lumina-landing.html',
  styleUrls: ['./lumina-landing.scss'],
})
export class LuminaLandingComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('heroCanvas') heroCanvasRef!: ElementRef<HTMLCanvasElement>;
  private router = inject(Router);
  private authService = inject(AuthService);

  readonly brand = BRAND;
  readonly nav = NAV;
  readonly hero = HERO_CONTENT;
  readonly featuresSection = FEATURES_SECTION;
  readonly dashboardSection = DASHBOARD_SECTION;
  readonly techSection = TECH_SECTION;
  readonly ctaSection = CTA_SECTION;
  readonly footerLinks: FooterLink[] = FOOTER_LINKS;
  readonly features: FeatureItem[] = FEATURES;
  readonly dashSidebarItems: DashSidebarItem[] = DASH_SIDEBAR_ITEMS;
  readonly dashboardStats: DashboardStat[] = DASHBOARD_STATS;
  readonly chartBars: ChartBar[] = CHART_BARS;
  readonly techColumns: TechColumn[] = TECH_COLUMNS;

  isNavScrolled = false;
  isHeroLoaderHidden = false;

  private ctx!: CanvasRenderingContext2D;
  private canvasWidth = 0;
  private canvasHeight = 0;
  private devicePixelRatioClamped = 1;

  private frameImages: HTMLImageElement[] = [];
  private useFrameSequence = false;
  private framesReady = false;

  private buildings: Building[] = [];
  private particles: Particle[] = [];
  private reduceMotion = false;

  private heroTimeline?: gsap.core.Timeline;
  private navScrollTrigger?: ScrollTrigger;
  private windowScrollHandler = () => this.onWindowScrollFallback();

  constructor(private sanitizer: DomSanitizer, private cdr: ChangeDetectorRef) {}

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/dashboard']);
    } 
  }

  ngAfterViewInit(): void {
    this.reduceMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    this.setupCanvas();
    this.buildProceduralScene();
    this.resizeCanvas();
    this.loadFrameSequenceOrFallback();
    this.cdr.detectChanges();
  }

  ngOnDestroy(): void {
    this.heroTimeline?.kill();
    this.navScrollTrigger?.kill();
    ScrollTrigger.getAll().forEach((trigger) => trigger.kill());
    window.removeEventListener('scroll', this.windowScrollHandler);
  }

  @HostListener('window:resize')
  onWindowResize(): void {
    this.resizeCanvas();
  }

  iconMarkup(svgInner: string): SafeHtml {
    return this.sanitizer.bypassSecurityTrustHtml(svgInner);
  }

  private setupCanvas(): void {
    const canvas = this.heroCanvasRef.nativeElement;
    this.ctx = canvas.getContext('2d') as CanvasRenderingContext2D;
  }

  private resizeCanvas(): void {
    const canvas = this.heroCanvasRef.nativeElement;
    this.devicePixelRatioClamped = Math.min(
      window.devicePixelRatio || 1,
      CANVAS_CONFIG.maxDevicePixelRatio,
    );
    this.canvasWidth = window.innerWidth;
    this.canvasHeight = window.innerHeight;
    canvas.style.width = `${this.canvasWidth}px`;
    canvas.style.height = `${this.canvasHeight}px`;
    canvas.width = this.canvasWidth * this.devicePixelRatioClamped;
    canvas.height = this.canvasHeight * this.devicePixelRatioClamped;
    this.ctx.setTransform(this.devicePixelRatioClamped, 0, 0, this.devicePixelRatioClamped, 0, 0);
  }

  private seededRandomFactory(seed: number): () => number {
    let s = seed;
    return () => {
      s = (s * 9301 + 49297) % 233280;
      return s / 233280;
    };
  }

  private buildProceduralScene(): void {
    const rnd = this.seededRandomFactory(CANVAS_CONFIG.randomSeed);

    this.buildings = Array.from({ length: CANVAS_CONFIG.buildingCount }, (_, i) => ({
      x: i / CANVAS_CONFIG.buildingCount,
      w: CANVAS_CONFIG.buildingWidthMin + rnd() * CANVAS_CONFIG.buildingWidthRange,
      h: CANVAS_CONFIG.buildingHeightMin + rnd() * CANVAS_CONFIG.buildingHeightRange,
      delay: rnd() * CANVAS_CONFIG.buildingDelayRange,
      windows:
        CANVAS_CONFIG.buildingWindowsMin + Math.floor(rnd() * CANVAS_CONFIG.buildingWindowsRange),
    }));

    this.particles = Array.from({ length: CANVAS_CONFIG.particleCount }, () => ({
      x: rnd(),
      y: rnd(),
      r: CANVAS_CONFIG.particleRadiusMin + rnd() * CANVAS_CONFIG.particleRadiusRange,
      speed: CANVAS_CONFIG.particleSpeedMin + rnd() * CANVAS_CONFIG.particleSpeedRange,
      phase: rnd() * Math.PI * 2,
    }));
  }

  private lerp(a: number, b: number, t: number): number {
    return a + (b - a) * t;
  }

  private clamp01(v: number): number {
    return Math.max(0, Math.min(1, v));
  }

  private renderProceduralScene(progress: number): void {
    const ctx = this.ctx;
    const W = this.canvasWidth;
    const H = this.canvasHeight;
    const emerald = CANVAS_CONFIG.colors.emeraldRgb;

    ctx.clearRect(0, 0, W, H);

    const g = ctx.createLinearGradient(0, 0, 0, H);
    const bgTop = `rgba(${this.lerp(251, 238, progress)}, ${this.lerp(253, 248, progress)}, ${this.lerp(252, 245, progress)}, 1)`;
    const bgBot = `rgba(${this.lerp(243, 222, progress)}, ${this.lerp(247, 238, progress)}, ${this.lerp(246, 233, progress)}, 1)`;
    g.addColorStop(0, bgTop);
    g.addColorStop(1, bgBot);
    ctx.fillStyle = g;
    ctx.fillRect(0, 0, W, H);

    const glowX = this.lerp(W * 0.2, W * 0.8, progress);
    const glow = ctx.createRadialGradient(glowX, H * 0.35, 0, glowX, H * 0.35, W * 0.4);
    glow.addColorStop(0, `rgba(${emerald},${0.1 + progress * 0.12})`);
    glow.addColorStop(1, `rgba(${emerald},0)`);
    ctx.fillStyle = glow;
    ctx.fillRect(0, 0, W, H);

    this.particles.forEach((p) => {
      const py = (((p.y - progress * p.speed) % 1) + 1) % 1;
      const px = p.x + Math.sin(progress * 6 + p.phase) * 0.01;
      ctx.beginPath();
      ctx.arc(px * W, py * H, p.r * (1 + progress), 0, Math.PI * 2);
      ctx.fillStyle = `rgba(${emerald},${0.15 + progress * 0.25})`;
      ctx.fill();
    });

    const baseline = H * CANVAS_CONFIG.skylineBaselineRatio;
    this.buildings.forEach((b) => {
      const bp = this.clamp01((progress - b.delay * 0.3) / 0.7);
      if (bp <= 0) {
        return;
      }
      const bw = b.w * W;
      const bh = b.h * H * bp;
      const bx = b.x * W;
      const by = baseline - bh;

      const fillAlpha = this.clamp01((bp - 0.35) / 0.65);
      ctx.beginPath();
      ctx.rect(bx, by, bw, bh);
      if (fillAlpha > 0) {
        const bg2 = ctx.createLinearGradient(0, by, 0, baseline);
        bg2.addColorStop(0, `rgba(${emerald},${0.55 * fillAlpha})`);
        bg2.addColorStop(1, `rgba(${emerald},${0.85 * fillAlpha})`);
        ctx.fillStyle = bg2;
        ctx.fill();
      }
      ctx.lineWidth = 1;
      ctx.strokeStyle = `rgba(${emerald},${0.5 + 0.3 * (1 - fillAlpha)})`;
      ctx.stroke();

      if (fillAlpha > 0.5) {
        const rows = b.windows;
        const cols = 2;
        const wAlpha = this.clamp01((fillAlpha - 0.5) / 0.5);
        for (let r = 0; r < rows; r++) {
          for (let c = 0; c < cols; c++) {
            const wx = bx + bw * (0.22 + c * 0.4);
            const wy = by + bh * (0.15 + r * (0.7 / rows));
            const lit = Math.sin(r * 3 + c * 7 + progress * 20) > 0.2;
            ctx.fillStyle = lit
              ? `rgba(${CANVAS_CONFIG.colors.windowLitRgb}, ${0.8 * wAlpha})`
              : `rgba(${CANVAS_CONFIG.colors.windowUnlitRgb}, ${0.35 * wAlpha})`;
            ctx.fillRect(wx, wy, bw * 0.14, bh * 0.08);
          }
        }
      }
    });

    ctx.beginPath();
    ctx.moveTo(0, baseline);
    ctx.lineTo(W, baseline);
    ctx.strokeStyle = `rgba(${emerald},0.25)`;
    ctx.lineWidth = 1;
    ctx.stroke();
  }
  loginpage(): void {
    this.router.navigate(['/login']);
  }

  registerpage(): void {
    this.router.navigate(['/register']);
  }

  private renderFrame(progress: number): void {
    if (this.useFrameSequence && this.frameImages.length) {
      const idx = Math.min(
        this.frameImages.length - 1,
        Math.floor(progress * (this.frameImages.length - 1)),
      );
      const img = this.frameImages[idx];
      if (img && img.complete) {
        const ctx = this.ctx;
        const W = this.canvasWidth;
        const H = this.canvasHeight;
        ctx.clearRect(0, 0, W, H);
        const ir = img.width / img.height;
        const cr = W / H;
        let dw: number, dh: number, dx: number, dy: number;
        if (ir > cr) {
          dh = H;
          dw = H * ir;
          dx = (W - dw) / 2;
          dy = 0;
        } else {
          dw = W;
          dh = W / ir;
          dx = 0;
          dy = (H - dh) / 2;
        }
        ctx.drawImage(img, dx, dy, dw, dh);
      }
    } else {
      this.renderProceduralScene(progress);
    }
  }

  private async detectFrameUrls(): Promise<string[]> {
    const candidates: string[] = [];
    let misses = 0;

    for (
      let i = 0;
      i < CANVAS_CONFIG.frameSearchLimit && misses < CANVAS_CONFIG.frameSearchMissTolerance;
      i++
    ) {
      const n = String(i).padStart(3, '0');
      let hit = false;
      for (const ext of CANVAS_CONFIG.frameExtensions) {
        const url = `${CANVAS_CONFIG.framesBasePath}frame${n}.${ext}`;
        const ok = await new Promise<boolean>((resolve) => {
          const img = new Image();
          img.onload = () => resolve(true);
          img.onerror = () => resolve(false);
          img.src = url;
        });
        if (ok) {
          candidates.push(url);
          hit = true;
          break;
        }
      }
      misses = hit ? 0 : misses + 1;
    }
    return candidates;
  }

  private loadFrameSequenceOrFallback(): void {
    this.detectFrameUrls().then((list) => {
      if (list.length > CANVAS_CONFIG.minFramesToUseSequence) {
        let loaded = 0;
        this.frameImages = list.map((src) => {
          const img = new Image();
          img.onload = img.onerror = () => {
            loaded++;
            if (loaded === list.length) {
              this.useFrameSequence = true;
              this.hideLoaderAndInitScroll();
            }
          };
          img.src = src;
          return img;
        });

        setTimeout(() => {
          if (!this.framesReady) {
            this.framesReady = true;
            this.useFrameSequence = this.frameImages.length > 0;
            this.hideLoaderAndInitScroll();
          }
        }, CANVAS_CONFIG.frameLoadTimeoutMs);
      } else {
        this.framesReady = true;
        setTimeout(() => this.hideLoaderAndInitScroll(), CANVAS_CONFIG.loaderHideDelayMs);
      }
    });
  }

  private hideLoaderAndInitScroll(): void {
    this.isHeroLoaderHidden = true;
    this.renderFrame(0);
    this.initScrollAnimations();
    this.cdr.detectChanges();
  }

  private initScrollAnimations(): void {
    if (this.reduceMotion) {
      this.renderFrame(0.5);
      this.revealStatic();
      return;
    }

    const cfg = SCROLL_ANIMATION_CONFIG.stages;

    this.heroTimeline = gsap.timeline({
      scrollTrigger: {
        trigger: '#hero-section',
        start: 'top top',
        end: SCROLL_ANIMATION_CONFIG.heroPinEnd,
        scrub: SCROLL_ANIMATION_CONFIG.scrubAmount,
        pin: '.hero-canvas-wrap',
        anticipatePin: 1,
        onUpdate: (self: ScrollTrigger) => this.renderFrame(self.progress),
      },
    });

    this.heroTimeline.to(
      '.hero-intro',
      { opacity: 0, y: -80, duration: cfg.introFadeOutDuration },
      cfg.introFadeOutAt,
    );
    this.heroTimeline.set('.hero-intro', { pointerEvents: 'none' }, cfg.introPointerNoneAt);

    this.heroTimeline.fromTo(
      '.stage-1',
      { opacity: 0, y: 40 },
      { opacity: 1, y: 0, duration: cfg.fadeDurationLong },
      cfg.stage1InAt,
    );
    this.heroTimeline.fromTo(
      '.stage-1 .stage-headline',
      { y: -30 },
      { y: 0, duration: cfg.fadeDurationLong },
      cfg.stage1HeadlineInAt,
    );
    this.heroTimeline.fromTo(
      '.stage-1 .stage-subtitle',
      { x: 40 },
      { x: 0, duration: cfg.fadeDurationLong },
      cfg.stage1SubtitleInAt,
    );
    this.heroTimeline.to(
      '.stage-1',
      { opacity: 0, y: -40, duration: cfg.fadeDurationMedium },
      cfg.stage1OutAt,
    );

    this.heroTimeline.fromTo(
      '.stage-2',
      { opacity: 0, y: 40 },
      { opacity: 1, y: 0, duration: cfg.fadeDurationLong },
      cfg.stage2InAt,
    );
    this.heroTimeline.to(
      '.stage-2',
      { opacity: 0, y: -40, duration: cfg.fadeDurationMedium },
      cfg.stage2OutAt,
    );

    this.heroTimeline.fromTo(
      '.stage-3',
      { opacity: 0, y: 60 },
      { opacity: 1, y: 0, duration: cfg.fadeDurationLong },
      cfg.stage3InAt,
    );
    this.heroTimeline.to(
      '.stage-3',
      { opacity: 0, y: -40, duration: cfg.fadeDurationMedium },
      cfg.stage3OutAt,
    );

    this.heroTimeline.fromTo(
      '.stage-4',
      { opacity: 0, y: 40 },
      { opacity: 1, y: 0, duration: cfg.fadeDurationXLong },
      cfg.stage4InAt,
    );
    this.heroTimeline.to(
      '.stage-4',
      { opacity: 0, duration: cfg.fadeDurationShort },
      cfg.stage4OutAt,
    );

    this.heroTimeline.to(
      '.scroll-cue',
      { opacity: 0, duration: cfg.fadeDurationShort },
      cfg.scrollCueFadeAt,
    );

    this.navScrollTrigger = ScrollTrigger.create({
      trigger: '#hero-section',
      start: SCROLL_ANIMATION_CONFIG.navScrolledTriggerStart,
      end: 'bottom top',
      onUpdate: (self: ScrollTrigger) => {
        this.isNavScrolled =
          self.progress > 0.02 || window.scrollY > SCROLL_ANIMATION_CONFIG.navScrollThresholdPx;
      },
    });

    window.addEventListener('scroll', this.windowScrollHandler);

    this.revealAnimated();
  }

  private onWindowScrollFallback(): void {
    if (window.scrollY < SCROLL_ANIMATION_CONFIG.navScrollThresholdPx) {
      this.isNavScrolled = false;
    }
  }

  private revealAnimated(): void {
    gsap.utils.toArray<HTMLElement>('.feature-card').forEach((card: HTMLElement, i: number) => {
      gsap.to(card, {
        opacity: 1,
        y: 0,
        duration: 0.7,
        ease: 'power3.out',
        delay:
          (i % SCROLL_ANIMATION_CONFIG.featureCardStaggerGroup) *
          SCROLL_ANIMATION_CONFIG.featureCardStaggerDelay,
        scrollTrigger: { trigger: card, start: SCROLL_ANIMATION_CONFIG.revealTriggerStart },
      });
    });

    gsap.to('#dashFrame', {
      opacity: 1,
      scale: 1,
      y: 0,
      duration: 1,
      ease: 'power3.out',
      scrollTrigger: {
        trigger: '#dashFrame',
        start: SCROLL_ANIMATION_CONFIG.dashboardTriggerStart,
      },
    });

    gsap.utils.toArray<HTMLElement>('.tech-col').forEach((col: HTMLElement, i: number) => {
      gsap.to(col, {
        opacity: 1,
        y: 0,
        duration: 0.7,
        ease: 'power3.out',
        delay: i * SCROLL_ANIMATION_CONFIG.techColStaggerDelay,
        scrollTrigger: { trigger: col, start: SCROLL_ANIMATION_CONFIG.techColTriggerStart },
      });
    });
  }

  private revealStatic(): void {
    document.querySelectorAll<HTMLElement>('.feature-card, .tech-col').forEach((el) => {
      el.style.opacity = '1';
      el.style.transform = 'none';
    });
    const df = document.getElementById('dashFrame');
    if (df) {
      df.style.opacity = '1';
      df.style.transform = 'none';
    }
  }
}
