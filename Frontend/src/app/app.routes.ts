import { Routes } from '@angular/router';
import { AuthLayoutComponent } from './layout/auth-layout/auth-layout';
import { MainLayoutComponent } from './layout/main-layout/main-layout';
import { DashboardPageComponent } from './features/dashboard/dashboard-page/dashboard-page';
import { ComplaintsListPage } from './features/complaints/pages/complaints-list-page/complaints-list-page';
import { CreateComplaintPage } from './features/complaints/pages/create-complaint-page/create-complaint-page';
import { ComplaintDetailPage } from './features/complaints/pages/complaint-detail-page/complaint-detail-page';
import { UserManagementPage } from './features/user-management/pages/user-management-page/user-management-page';
import { VisitsListPage } from './features/visitor-management/pages/visits-list-page/visits-list-page';
import { VisitDetailPage } from './features/visitor-management/pages/visit-detail-page/visit-detail-page';
import { BookUnplannedVisitPage } from './features/visitor-management/pages/book-unplanned-visit-page/book-unplanned-visit-page';
import { ScanVisitorPage } from './features/visitor-management/pages/scan-visitor-page/scan-visitor-page';
import { SecurityVisitorsPage } from './features/visitor-management/pages/security-visitors-page/security-visitors-page';
import { SettingsPageComponent } from './features/settings/settings-page/settings-page';
import { authGuard } from './core/guards/auth.guard';
import { AmenityDashboardUser } from './features/amenity/pages/amenity-dashboard-user/amenity-dashboard-user';
import { AmenityBookingPage } from './features/amenity/pages/amenity-booking-page/amenity-booking-page';
import { AmenityBookingsListPage } from './features/amenity/pages/amenity-bookings-list-page/amenity-bookings-list-page';
import { AmenityCancellationBookingPage } from './features/amenity/pages/amenity-cancellation-booking-page/amenity-cancellation-booking-page';
import { AddNewAmenityPage } from './features/amenity/pages/add-new-amenity-page/add-new-amenity-page';
import { requirePermission } from './core/guards/permission.guard';
import { PERMISSIONS } from './core/constants/permission.constants';
import { requireRole } from './core/guards/role.guard';
import { APP_CONSTANTS } from './core/constants/app.constants';
import { LuminaLandingComponent } from './features/lumina-landing/lumina-landing';

const R = APP_CONSTANTS.ROUTES;
export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: 'home', component: LuminaLandingComponent },
  {
    path: '',
    component: AuthLayoutComponent,
    loadChildren: () => import('./features/auth/auth.routes').then((m) => m.AUTH_FEATURE_ROUTES),
  },
  {
    path: '',
    component: MainLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardPageComponent },
      { path: 'complaints', component: ComplaintsListPage },
      { path: 'complaints/new', component: CreateComplaintPage },
      { path: 'complaints/:complaintId', component: ComplaintDetailPage },
      { path: 'amenities', component: AmenityDashboardUser },
      { path: 'amenities/new', component: AddNewAmenityPage },
      { path: 'amenities/bookings', component: AmenityBookingsListPage },
      { path: 'amenities/bookings/:bookingId/cancel', component: AmenityCancellationBookingPage },
      { path: 'amenities/:amenityId/book', component: AmenityBookingPage },
      { path: '', redirectTo: R.DASHBOARD.slice(1), pathMatch: 'full' },
      { path: R.DASHBOARD.slice(1), component: DashboardPageComponent },
      { path: R.SETTINGS.slice(1), component: SettingsPageComponent },
      { path: R.COMPLAINTS.slice(1), component: ComplaintsListPage },
      { path: R.COMPLAINTS.slice(1) + '/new', component: CreateComplaintPage },
      { path: R.COMPLAINTS.slice(1) + '/:complaintId', component: ComplaintDetailPage },
      {
        path: R.USER_MANAGEMENT.slice(1),
        component: UserManagementPage,
        canActivate: [requirePermission(PERMISSIONS.USER_MANAGE)],
      },
      { path: 'visitors', component: VisitsListPage },
      { path: 'visitors/:visitId', component: VisitDetailPage },
      {
        path: 'security/book-visitor',
        component: BookUnplannedVisitPage,
        canActivate: [requirePermission(PERMISSIONS.VISIT_REGISTER)],
      },
      {
        path: 'security/scan',
        component: ScanVisitorPage,
        canActivate: [requirePermission(PERMISSIONS.VISIT_CHECKIN)],
      },
      {
        path: 'security/visitors',
        component: SecurityVisitorsPage,
        canActivate: [requirePermission(PERMISSIONS.VISIT_VIEW)],
      },
    ],
  },
];
