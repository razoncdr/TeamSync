import { Component, HostListener, OnInit } from '@angular/core';
import { CommonModule, NgIf, NgFor } from '@angular/common';
import { NotificationService } from '../../services/notification';
import { Notification } from '../../models/notification';
import { NotificationHubService } from '../../services/notification-hub';

@Component({
  selector: 'app-notification-bell',
  standalone: true,
  imports: [CommonModule, NgIf, NgFor],
  templateUrl: './notification-bell.html',
  styleUrls: ['./notification-bell.css']
})
export class NotificationBellComponent implements OnInit {
  notifications: Notification[] = [];
  isOpen = false;
  unreadCount = 0;

  constructor(private notificationService: NotificationService, private notificationHub: NotificationHubService) {}

  ngOnInit() {
    this.loadNotifications();

    this.notificationHub.start((notification) => {
      this.notifications.unshift(notification);
      this.unreadCount++;
    });
  }

    ngOnDestroy() {
      this.notificationHub.stop();
    }


  loadNotifications() {
    this.notificationService.getMyNotifications().subscribe(res => {
      this.notifications = res;
      this.unreadCount = res.filter(n => !n.isRead).length;
    });
  }

  toggle() {
    this.isOpen = !this.isOpen;
  }

  markAsRead(notification: Notification) {
    if (notification.isRead) return;

    this.notificationService.markAsRead(notification.id).subscribe(() => {
      notification.isRead = true;
      this.unreadCount--;
    });
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe(() => {
      this.notifications.forEach(n => (n.isRead = true));
      this.unreadCount = 0;
    });
  }

  @HostListener('document:click', ['$event'])
  closeOnOutsideClick(event: Event) {
    const target = event.target as HTMLElement;
    if (!target.closest('.notification-wrapper')) {
      this.isOpen = false;
    }
  }
}
