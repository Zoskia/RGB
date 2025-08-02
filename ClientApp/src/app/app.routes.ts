import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { HexGridComponent } from './features/hex-grid/hex-grid.component';
import { HexTestComponent } from './features/hex-test/hex-test.component';
import { AuthGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: 'login',
    component: LoginComponent,
    title: 'Login',
  },
  {
    path: 'register',
    component: RegisterComponent,
    title: 'Register',
  },
  {
    path: 'hex-grid',
    component: HexGridComponent,
    title: 'HexGrid',
    canActivate: [AuthGuard],
  },
  {
    path: 'hex-test',
    component: HexTestComponent,
    title: 'HexTest',
    canActivate: [AuthGuard],
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }, // fallback
];
