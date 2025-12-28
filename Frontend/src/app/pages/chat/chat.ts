import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChatService, Notification } from '../../services/chat';
import { ChatHubService } from '../../services/chat-hub';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  projectId!: string;
  currentUserId!: string;
  messages: Notification[] = [];

  private readonly pageSize = 20;
  private skip = 0;
  hasMore = true;
  loadingOlder = false;

  constructor(
    private route: ActivatedRoute,
    private chatService: ChatService,
    private chatHub: ChatHubService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.currentUserId = this.getUserIdFromToken();
    this.loadInitialMessages();

    this.chatHub.start(this.projectId, (msg) => {
      this.messages.push(msg);
    });
  }

  ngOnDestroy(): void {
    this.chatHub.stop(this.projectId);
  }

  loadInitialMessages(): void {
    this.skip = 0;
    this.chatService
      .getChatMessages(this.projectId, this.skip, this.pageSize)
      .subscribe({
        next: (res) => {
          this.messages = res.data;
          this.skip += this.pageSize;
          this.hasMore = res.data.length === this.pageSize;
        },
        error: (err) => {
          console.error(err);
          this.toastr.error('Failed to load chat messages', 'Error');
        }
      });
  }

  loadPreviousMessages(): void {
    if (!this.hasMore || this.loadingOlder) return;

    this.loadingOlder = true;

    this.chatService
      .getChatMessages(this.projectId, this.skip, this.pageSize)
      .subscribe({
        next: (res) => {
          this.messages = [...res.data, ...this.messages];
          this.skip += this.pageSize;
          this.hasMore = res.data.length === this.pageSize;
          this.loadingOlder = false;
        },
        error: (err) => {
          console.error(err);
          this.toastr.error('Failed to load older messages', 'Error');
          this.loadingOlder = false;
        },
      });
  }

  private getUserIdFromToken(): string {
  const token = localStorage.getItem('token');
  if (!token) return '';

  const payload = JSON.parse(atob(token.split('.')[1]));
  return payload.sub || payload.nameid;
}

  sendMessage(messageInput: HTMLInputElement): void {
    const text = messageInput.value.trim();
    if (!text) return;

    this.chatService.sendMessage(this.projectId, text).subscribe({
      next: () => (messageInput.value = ''),
      error: (err) => {
        console.error(err);
        this.toastr.error('Failed to send message', 'Error');
      },
    });
  }
}
