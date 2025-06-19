import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login/login';
import { RegisterComponent } from './auth/register/register';
import { DashboardComponent } from './dashboard/dashboard';
import { LayoutComponent } from './layout/layout';
import { UserListComponent } from './auth/user-list/user-list';
import { RoleListComponent } from './auth/role-list/role-list';
import { AuthGuard } from './guard/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'dashboard',
    canActivate: [AuthGuard],
    component: LayoutComponent,
    children: [
      { path: '', component: DashboardComponent },
      { path: 'users', component: UserListComponent },
      {
        path: 'roles',
        component: RoleListComponent,
      },
      // other protected routes like /users, /roles
    ],
  },
];
