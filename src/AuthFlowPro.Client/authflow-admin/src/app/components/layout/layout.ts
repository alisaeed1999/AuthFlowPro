import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { RouterOutlet } from '@angular/router';
import { MatMenuModule } from '@angular/material/menu';
import { NotificationsComponent } from './notifications/notifications.component';
import { SignalRService } from '../../services/signalr.service';
import { AuthService } from '../../services/auth-service';

@Component({
  selector: 'app-layout',
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    MatToolbarModule,
    MatIconModule,
    MatListModule,
    MatButtonModule,
    RouterOutlet,
    MatMenuModule,
    NotificationsComponent
  ],
  templateUrl: './layout.html',
  styleUrl: './layout.css'
})
export class LayoutComponent implements OnInit, OnDestroy {

  userName : string = '';
  
  constructor(private signalRService: SignalRService , private authService : AuthService) {
    const user = this.authService.getCurrentUser();
    if(user){
      this.userName = user.userName;
      console.log(user);
      
    }
  }

  ngOnInit(): void {
    // Initialize SignalR connection when layout loads
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    // Clean up SignalR connection when component is destroyed
    this.signalRService.stopConnection();
  }

  logout() {
    // Stop SignalR connection before logout
    this.signalRService.stopConnection();
    
    // Remove token and redirect
    localStorage.removeItem('access_token');
    window.location.href = '/login';
  }
}