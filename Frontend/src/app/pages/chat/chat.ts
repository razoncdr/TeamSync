import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ChatService, ChatMessage } from '../../services/chat';
import { Subscription, interval } from 'rxjs';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './chat.html',
  styleUrl: './chat.css',
})
export class ChatComponent implements OnInit, OnDestroy {
  projectId!: string;
  messages: ChatMessage[] = [];

  private pollingSub?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private chatService: ChatService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.loadChat();

    // TEMP polling (remove after SignalR)
    // this.pollingSub = interval(5000).subscribe(() => {
    //   this.loadChat();
    // });
  }

  ngOnDestroy(): void {
    this.pollingSub?.unsubscribe();
  }

  loadChat(): void {
    this.chatService.getChatMessages(this.projectId).subscribe({
      next: (res) => (this.messages = res.data),
      error: (err) => console.error('Failed to load chat messages', err),
    });
  }

  sendMessage(messageInput: HTMLInputElement): void {
    const text = messageInput.value.trim();
    if (!text) return;

    this.chatService.sendMessage(this.projectId, text).subscribe({
      next: (res) => {
        this.messages.push(res.data);
        messageInput.value = '';
      },
      error: (err) => console.error('Failed to send message', err),
    });
  }
}
