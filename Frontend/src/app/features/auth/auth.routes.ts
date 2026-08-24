import { Routes } from '@angular/router';
import { AUTH_ROUTES } from '../../core/constants/auth.constants';
import { LuminaLandingComponent } from '../lumina-landing/lumina-landing';

export const AUTH_FEATURE_ROUTES: Routes = [
  {
    path: AUTH_ROUTES.LOGIN,
    loadComponent: () => import('./pages/login-page/login-page').then((m) => m.LoginPage)
  },
  {
    path: AUTH_ROUTES.REGISTER,
    loadComponent: () => import('./pages/register-page/register-page').then((m) => m.RegisterPage)
  },
  {
    path: AUTH_ROUTES.FORGOT_PASSWORD,
    loadComponent: () => import('./pages/forgot-password-page/forgot-password-page').then((m) => m.ForgotPasswordPage)
  },
  {
    path: AUTH_ROUTES.RESET_PASSWORD,
    loadComponent: () => import('./pages/reset-password-page/reset-password-page').then((m) => m.ResetPasswordPage)
  },
    {path : 'landing' , component : LuminaLandingComponent},
];