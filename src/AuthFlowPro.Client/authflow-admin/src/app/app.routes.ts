import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login'; 
import { RegisterComponent } from './components/auth/register/register';  
import { LayoutComponent } from './components/layout/layout'; 
import { UserListComponent } from './components/layout/user-list/user-list';
import { RoleListComponent } from './components/layout/role-list/role-list';
import { OrganizationsComponent } from './components/layout/organizations/organizations.component';
import { AuthGuard } from './guard/auth-guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'admin',
    canActivate: [AuthGuard],
    component: LayoutComponent,
    children: [
      { path: 'users', component: UserListComponent },
      { path: 'roles', component: RoleListComponent },
      { path: 'organizations', component: OrganizationsComponent },
      { path: 'subscriptions', loadComponent: () => import('./components/layout/subscriptions/subscriptions.component').then(m => m.SubscriptionsComponent) },
      { path: 'audit-logs', loadComponent: () => import('./components/layout/audit-logs/audit-logs.component').then(m => m.AuditLogsComponent) },
    ],
  },
];