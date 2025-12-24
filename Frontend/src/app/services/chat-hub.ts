import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Notification } from './chat';

@Injectable({ providedIn: 'root' })
export class ChatHubService {
  private connection!: signalR.HubConnection;

  start(projectId: string, onMessage: (msg: Notification) => void) {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7035/hubs/chat', {
        accessTokenFactory: () => localStorage.getItem('token') || '',
      })
      .withAutomaticReconnect()
      .build();

    this.connection.on('ReceiveMessage', onMessage);

    this.connection.start().then(() => {
      this.connection.invoke('JoinProject', projectId);
    });
  }

  stop(projectId: string) {
    if (!this.connection) return;

    this.connection.invoke('LeaveProject', projectId);
    this.connection.stop();
  }
}
