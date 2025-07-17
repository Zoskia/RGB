import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { HexGridComponent } from './features/hexgrid/hexgrid.component';

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
    path: 'hexgrid',
    component: HexGridComponent,
    title: 'HexGrid',
  },
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '**', redirectTo: 'login' }, // fallback
];
