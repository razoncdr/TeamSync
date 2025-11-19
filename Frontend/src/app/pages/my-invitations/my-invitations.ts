import { Component, OnInit } from '@angular/core';
import { MemberService } from '../../services/member';
import { NgIf, NgFor, DatePipe, NgClass } from '@angular/common';

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

  constructor(private memberService: MemberService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.loading = true;

    this.memberService.getMyInvitations().subscribe({
      next: (res) => {
        this.invitations = res.data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      },
    });
  }

  accept(id: string) {
    this.memberService.acceptInvitation(id).subscribe({
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
    this.memberService.rejectInvitation(id).subscribe({
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
