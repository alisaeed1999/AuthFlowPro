import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { SignalRService, NotificationMessage } from '../../../services/signalr.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatBadgeModule,
    MatListModule,
    MatDividerModule
  ],
  template: `
    <button 
      mat-icon-button 
      [matMenuTriggerFor]="notificationMenu"
      [matBadge]="unreadCount"
      [matBadgeHidden]="unreadCount === 0"
      matBadgeColor="warn">
      <mat-icon>notifications</mat-icon>
    </button>

    <mat-menu #notificationMenu="matMenu" class="notification-menu">
      <div class="notification-header" (click)="$event.stopPropagation()">
        <h3>Notifications</h3>
        <button mat-button (click)="clearAll()" *ngIf="notifications.length > 0">
          Clear All
        </button>
      </div>
      <mat-divider></mat-divider>
      
      <div class="notification-list" *ngIf="notifications.length > 0; else noNotifications">
        <div 
          *ngFor="let notification of notifications; trackBy: trackByNotificationId"
          class="notification-item"
          [class]="'notification-' + notification.type"
          (click)="handleNotificationClick(notification)">
          
          <div class="notification-content">
            <div class="notification-title">{{ notification.title }}</div>
            <div class="notification-message">{{ notification.message }}</div>
            <div class="notification-time">{{ getTimeAgo(notification.timestamp) }}</div>
          </div>
          
          <button 
            mat-icon-button 
            class="notification-close"
            (click)="removeNotification(notification.id, $event)">
            <mat-icon>close</mat-icon>
          </button>
        </div>
      </div>

      <ng-template #noNotifications>
        <div class="no-notifications">
          <mat-icon>notifications_none</mat-icon>
          <p>No notifications</p>
        </div>
      </ng-template>
    </mat-menu>
  `,
  styles: [`
    .notification-menu {
      width: 350px;
      max-height: 400px;
    }

    .notification-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      padding: 16px;
      background-color: #f5f5f5;
    }

    .notification-header h3 {
      margin: 0;
      font-size: 16px;
      font-weight: 500;
    }

    .notification-list {
      max-height: 300px;
      overflow-y: auto;
    }

    .notification-item {
      display: flex;
      padding: 12px 16px;
      border-bottom: 1px solid #eee;
      cursor: pointer;
      transition: background-color 0.2s;
    }

    .notification-item:hover {
      background-color: #f9f9f9;
    }

    .notification-content {
      flex: 1;
    }

    .notification-title {
      font-weight: 500;
      font-size: 14px;
      margin-bottom: 4px;
    }

    .notification-message {
      font-size: 12px;
      color: #666;
      margin-bottom: 4px;
    }

    .notification-time {
      font-size: 11px;
      color: #999;
    }

    .notification-close {
      width: 24px;
      height: 24px;
      line-height: 24px;
    }

    .notification-info .notification-title {
      color: #2196f3;
    }

    .notification-success .notification-title {
      color: #4caf50;
    }

    .notification-warning .notification-title {
      color: #ff9800;
    }

    .notification-error .notification-title {
      color: #f44336;
    }

    .no-notifications {
      text-align: center;
      padding: 32px 16px;
      color: #999;
    }

    .no-notifications mat-icon {
      font-size: 48px;
      width: 48px;
      height: 48px;
      margin-bottom: 8px;
    }
  `]
})
export class NotificationsComponent implements OnInit, OnDestroy {
  notifications: NotificationMessage[] = [];
  unreadCount = 0;
  private subscription?: Subscription;

  constructor(private signalRService: SignalRService) {}

  ngOnInit(): void {
    this.subscription = this.signalRService.notifications$.subscribe(notifications => {
      this.notifications = notifications.slice(0, 10); // Show only latest 10
      this.unreadCount = notifications.length;
    });

    // Start SignalR connection
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  trackByNotificationId(index: number, notification: NotificationMessage): string {
    return notification.id;
  }

  removeNotification(notificationId: string, event: Event): void {
    event.stopPropagation();
    this.signalRService.removeNotification(notificationId);
  }

  clearAll(): void {
    this.signalRService.clearNotifications();
  }

  handleNotificationClick(notification: NotificationMessage): void {
    if (notification.actionUrl) {
      // Navigate to the action URL
      window.open(notification.actionUrl, '_blank');
    }
    this.removeNotification(notification.id, new Event('click'));
  }

  getTimeAgo(timestamp: Date): string {
    const now = new Date();
    const diff = now.getTime() - new Date(timestamp).getTime();
    const minutes = Math.floor(diff / 60000);
    
    if (minutes < 1) return 'Just now';
    if (minutes < 60) return `${minutes}m ago`;
    
    const hours = Math.floor(minutes / 60);
    if (hours < 24) return `${hours}h ago`;
    
    const days = Math.floor(hours / 24);
    return `${days}d ago`;
  }
}