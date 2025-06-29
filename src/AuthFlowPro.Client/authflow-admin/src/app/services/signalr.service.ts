import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { AuthService } from './auth-service';

export interface NotificationMessage {
  id: string;
  title: string;
  message: string;
  type: 'info' | 'success' | 'warning' | 'error';
  timestamp: Date;
  actionUrl?: string;
}

@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  private hubConnection: HubConnection | null = null;
  private notificationsSubject = new BehaviorSubject<NotificationMessage[]>([]);
  private connectionStateSubject = new BehaviorSubject<boolean>(false);

  public notifications$ = this.notificationsSubject.asObservable();
  public connectionState$ = this.connectionStateSubject.asObservable();

  constructor(private authService: AuthService) {}

  public async startConnection(): Promise<void> {
    if (this.hubConnection?.state === 'Connected') {
      return;
    }

    const token = this.authService.getToken();
    if (!token) {
      console.warn('No token available for SignalR connection');
      return;
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:5063/notificationHub', {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    try {
      await this.hubConnection.start();
      console.log('SignalR Connected');
      this.connectionStateSubject.next(true);
      this.setupEventListeners();
    } catch (err) {
      console.error('Error while starting SignalR connection: ', err);
      this.connectionStateSubject.next(false);
    }
  }

  public async stopConnection(): Promise<void> {
    if (this.hubConnection) {
      await this.hubConnection.stop();
      this.connectionStateSubject.next(false);
      console.log('SignalR Disconnected');
    }
  }

  public async joinOrganizationGroup(organizationId: string): Promise<void> {
    if (this.hubConnection?.state === 'Connected') {
      await this.hubConnection.invoke('JoinOrganizationGroup', organizationId);
    }
  }

  public async leaveOrganizationGroup(organizationId: string): Promise<void> {
    if (this.hubConnection?.state === 'Connected') {
      await this.hubConnection.invoke('LeaveOrganizationGroup', organizationId);
    }
  }

  private setupEventListeners(): void {
    if (!this.hubConnection) return;

    this.hubConnection.on('ReceiveNotification', (notification: NotificationMessage) => {
      const currentNotifications = this.notificationsSubject.value;
      this.notificationsSubject.next([notification, ...currentNotifications]);
    });

    this.hubConnection.on('UserJoined', (message: string) => {
      console.log('User joined:', message);
    });

    this.hubConnection.on('UserLeft', (message: string) => {
      console.log('User left:', message);
    });

    this.hubConnection.onreconnected(() => {
      console.log('SignalR Reconnected');
      this.connectionStateSubject.next(true);
    });

    this.hubConnection.onreconnecting(() => {
      console.log('SignalR Reconnecting...');
      this.connectionStateSubject.next(false);
    });

    this.hubConnection.onclose(() => {
      console.log('SignalR Connection Closed');
      this.connectionStateSubject.next(false);
    });
  }

  public clearNotifications(): void {
    this.notificationsSubject.next([]);
  }

  public removeNotification(notificationId: string): void {
    const currentNotifications = this.notificationsSubject.value;
    const updatedNotifications = currentNotifications.filter(n => n.id !== notificationId);
    this.notificationsSubject.next(updatedNotifications);
  }
}