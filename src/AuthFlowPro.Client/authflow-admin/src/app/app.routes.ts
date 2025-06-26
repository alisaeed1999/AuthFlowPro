import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login'; 
import { RegisterComponent } from './components/auth/register/register'; 
import { DashboardComponent } from './components/layout/dashboard/dashboard'; 
import { LayoutComponent } from './components/layout/layout'; 
import { UserListComponent } from './components/layout/user-list/user-list';
import { RoleListComponent } from './components/layout/role-list/role-list';
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
      // { path: '', component: DashboardComponent },
      { path: 'users', component: UserListComponent },
      {
        path: 'roles',
        component: RoleListComponent,
      },
      // other protected routes like /users, /roles
    ],
  },
];
