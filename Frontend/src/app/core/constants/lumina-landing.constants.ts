export interface FeatureItem {
  icon: string;
  title: string;
  description: string;
}

export interface TechColumn {
  index: string;
  title: string;
  items: string[];
}

export interface DashboardStat {
  value: string;
  label: string;
}

export interface DashSidebarItem {
  label: string;
  active: boolean;
}

export interface ChartBar {
  heightPercent: number;
}

export interface FooterLink {
  label: string;
  href: string;
}

export const BRAND = {
  name: 'Lumina Community',
  systemName: 'Smart Apartment Community Management System',
  pageTitle: 'Lumina Community — Smart Apartment Community Management System',
  metaDescription:
    'Manage your apartment, amenities, visitors, maintenance, and community — all from one modern web platform.'
};

export const NAV = {
  ctaLabel: 'Login',
  ctaLabel1: 'Register',
  ctaHref: '#cta'
};

export const HERO_CONTENT = {
  loaderText: 'Loading Platform Experience...',
  scrollCueLabel: 'Scroll',
  stage1: {
    headline: 'Smart Living.',
    subtitle: 'Connected Community.'
  },
  stage2: {
    headlinePrefix: 'Everything Your ',
    headlineEmphasis: 'Community',
    headlineSuffix: ' Needs.'
  },
  stage3List: [
    'Visitor Management',
    'Amenity Booking',
    'Maintenance',
    'Community Notices',
    'Resident Services'
  ],
  stage4: {
    headlineLine1: 'One Platform.',
    headlineLine2: 'One Community.'
  }
};

export const FEATURES_SECTION = {
  eyebrow: 'Platform Capabilities',
  heading: 'Every part of community life, in one place',
  description:
    'From the front gate to the front desk, Lumina brings residents, staff, and services onto a single connected system.'
};

export const DASHBOARD_SECTION = {
  eyebrow: 'The Platform',
  heading: 'One dashboard, every community function',
  description:
    'A modern enterprise web application built for administrators and residents alike.',
  browserUrl: 'app.luminacommunity.com/dashboard',
  mainHeading: 'Overview',
  sidebarBrand: 'Lumina'
};

export const TECH_SECTION = {
  eyebrow: 'Built For Everyone',
  heading: 'Designed around every role in the community'
};

export const CTA_SECTION = {
  heading: 'Ready to Transform Your Community?',
  description: 'Experience modern apartment management through one intelligent web platform.',
  buttonLabel: 'Login',
  buttonHref: '#hero-section'
};

export const FOOTER_LINKS: FooterLink[] = [
  { label: 'Privacy', href: '#' },
  { label: 'Terms', href: '#' },
  { label: 'Contact', href: '#' }
];

export const ICONS = {
  visitors:
    '<path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M22 21v-2a4 4 0 0 0-3-3.87"/><path d="M16 3.13a4 4 0 0 1 0 7.75"/>',
  amenity:
    '<rect x="3" y="4" width="18" height="18" rx="3"/><path d="M16 2v4M8 2v4M3 10h18"/>',
  maintenance:
    '<path d="M14.7 6.3a1 1 0 0 0 0 1.4l1.6 1.6a1 1 0 0 0 1.4 0l3.77-3.77a6 6 0 0 1-7.94 7.94l-6.91 6.91a2.12 2.12 0 0 1-3-3l6.91-6.91a6 6 0 0 1 7.94-7.94l-3.76 3.76z"/>',
  notices:
    '<path d="M18 8a6 6 0 0 0-12 0c0 7-3 9-3 9h18s-3-2-3-9"/><path d="M13.73 21a2 2 0 0 1-3.46 0"/>',
  parking:
    '<path d="M9 17h6M9 12h6M5 21V7l7-4 7 4v14"/>',
  directory:
    '<path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/><circle cx="9" cy="7" r="4"/><path d="M23 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75"/>'
};

export const FEATURES: FeatureItem[] = [
  {
    icon: ICONS.visitors,
    title: 'Visitor Management',
    description: 'Secure visitor approvals with QR access.'
  },
  {
    icon: ICONS.amenity,
    title: 'Amenity Booking',
    description: 'Reserve shared facilities effortlessly.'
  },
  {
    icon: ICONS.maintenance,
    title: 'Maintenance Requests',
    description: 'Track issues in real time.'
  },
  {
    icon: ICONS.notices,
    title: 'Community Notices',
    description: 'Instant announcements.'
  },
  {
    icon: ICONS.parking,
    title: 'Parking Management',
    description: 'Manage resident and visitor parking.'
  },
  {
    icon: ICONS.directory,
    title: 'Resident Directory',
    description: 'Connect with your community securely.'
  }
];

export const DASH_SIDEBAR_ITEMS: DashSidebarItem[] = [
  { label: 'Dashboard', active: true },
  { label: 'Visitors', active: false },
  { label: 'Amenities', active: false },
  { label: 'Bookings', active: false },
  { label: 'Complaints', active: false },
  { label: 'Community Notices', active: false },
  { label: 'Resident Directory', active: false },
  { label: 'Analytics', active: false },
  { label: 'Settings', active: false }
];

export const DASHBOARD_STATS: DashboardStat[] = [
  { value: '128', label: 'Active Residents' },
  { value: '34', label: 'Visitors Today' },
  { value: '9', label: 'Open Requests' },
  { value: '96%', label: 'Amenity Uptime' }
];

export const CHART_BARS: ChartBar[] = [
  { heightPercent: 40 },
  { heightPercent: 65 },
  { heightPercent: 50 },
  { heightPercent: 80 },
  { heightPercent: 60 },
  { heightPercent: 90 },
  { heightPercent: 45 }
];

export const TECH_COLUMNS: TechColumn[] = [
  {
    index: 'I.',
    title: 'Resident Experience',
    items: ['Easy visitor approvals', 'Book amenities', 'Receive notices']
  },
  {
    index: 'II.',
    title: 'Management Portal',
    items: ['Approve requests', 'Monitor bookings', 'Manage complaints']
  },
  {
    index: 'III.',
    title: 'Community Security',
    items: ['Controlled access', 'Verified visitors', 'Secure communication']
  }
];

export const CANVAS_CONFIG = {
  framesBasePath: 'frames/',
  frameSearchLimit: 300,
  frameSearchMissTolerance: 6,
  frameExtensions: ['jpg', 'png', 'webp'] as const,
  frameLoadTimeoutMs: 4000,
  loaderHideDelayMs: 350,
  minFramesToUseSequence: 4,
  maxDevicePixelRatio: 2,
  buildingCount: 14,
  particleCount: 40,
  randomSeed: 42,
  buildingHeightMin: 0.18,
  buildingHeightRange: 0.5,
  buildingWidthMin: 0.035,
  buildingWidthRange: 0.03,
  buildingDelayRange: 0.5,
  buildingWindowsMin: 3,
  buildingWindowsRange: 4,
  particleRadiusMin: 0.6,
  particleRadiusRange: 1.6,
  particleSpeedMin: 0.15,
  particleSpeedRange: 0.3,
  skylineBaselineRatio: 0.82,
  colors: {
    emeraldRgb: '15,118,110',
    windowLitRgb: '255, 220, 150',
    windowUnlitRgb: '255,255,255'
  }
};

// ------------------------------------------------------------
// GSAP ScrollTrigger timing configuration
// ------------------------------------------------------------
export const SCROLL_ANIMATION_CONFIG = {
  heroPinEnd: '+=350%',
  scrubAmount: 2,
  stages: {
    introFadeOutAt: 0,
    introFadeOutDuration: 1,
    introPointerNoneAt: 0.4,
    stage1InAt: 0.9,
    stage1HeadlineInAt: 0.9,
    stage1SubtitleInAt: 1.0,
    stage1OutAt: 2.1,
    stage2InAt: 2.4,
    stage2OutAt: 3.6,
    stage3InAt: 3.9,
    stage3OutAt: 5.4,
    stage4InAt: 5.7,
    stage4OutAt: 6.8,
    scrollCueFadeAt: 0.3,
    fadeDurationShort: 0.4,
    fadeDurationMedium: 0.6,
    fadeDurationLong: 0.8,
    fadeDurationXLong: 0.9
  },
  featureCardStaggerGroup: 3,
  featureCardStaggerDelay: 0.08,
  techColStaggerDelay: 0.1,
  navScrollThresholdPx: 60,
  navScrolledTriggerStart: 'top+=80 top',
  revealTriggerStart: 'top 88%',
  dashboardTriggerStart: 'top 82%',
  techColTriggerStart: 'top 85%'
};
