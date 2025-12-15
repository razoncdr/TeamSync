import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor, DatePipe, NgClass } from '@angular/common';
import { InvitationService } from '../../services/invitation';

@Component({
  selector: 'app-my-invitations',
  standalone: true,
  templateUrl: './my-invitations.html',
  styleUrls: ['./my-invitations.css'],
  imports: [NgIf, NgFor, NgClass, DatePipe],
})
export class MyInvitations implements OnInit {
  invitations: any[] = [];
  loading = true;

  constructor(private invitationService: InvitationService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;

    this.invitationService.getMyInvitations().subscribe({
      next: (res) => {
        this.invitations = res.data || [];
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      },
    });
  }

  accept(id: string) {
    this.invitationService.acceptInvitation(id).subscribe({
      next: () => {
        alert('Invitation accepted');
        this.load();
      },
      error: (err) => {
        alert(err.error?.message || 'Error');
      },
    });
  }

  reject(id: string) {
    this.invitationService.rejectInvitation(id).subscribe({
      next: () => {
        alert('Invitation rejected');
        this.load();
      },
      error: (err) => {
        alert(err.error?.message || 'Error');
      },
    });
  }
}
