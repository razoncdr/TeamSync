import { Component, OnInit } from '@angular/core';
import { NgIf, NgFor, DatePipe, NgClass } from '@angular/common';
import { InvitationService } from '../../services/invitation';
import { ToastrService } from 'ngx-toastr';

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

  constructor(private invitationService: InvitationService, private toastr: ToastrService) {}

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
        this.toastr.error('Failed to load invitations', 'Error');
        this.loading = false;
      },
    });
  }

  accept(id: string) {
    this.invitationService.acceptInvitation(id).subscribe({
      next: () => {
this.toastr.success('Invitation accepted', 'Success');        this.load();
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Error', 'Error');
      },
    });
  }

  reject(id: string) {
    this.invitationService.rejectInvitation(id).subscribe({
      next: () => {
        this.toastr.success('Invitation rejected', 'Success');
        this.load();
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Error', 'Error');
      },
    });
  }
}
