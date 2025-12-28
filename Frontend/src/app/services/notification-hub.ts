import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Notification } from '../models/notification';

@Injectable({ providedIn: 'root' })
export class NotificationHubService {
  private connection!: signalR.HubConnection;

  start(
    onNotification: (notification: Notification) => void
  ) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7035/hubs/notifications', {
        accessTokenFactory: () => localStorage.getItem('token') || '',
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on(
      'ReceiveNotification',
      (title: string, message: string, data: any) => {
        onNotification({
          id: crypto.randomUUID(),
          title,
          message,
          type: data?.type ?? 'task',
          isRead: false,
          createdAt: new Date().toISOString()
        });
      }
    );

    this.connection.start()
      .then(() => {
        console.log('Notification hub connected');
        this.connection.invoke('JoinUser');
      });
  }

  stop() {
    if (!this.connection) return;
    this.connection.stop();
  }
}
