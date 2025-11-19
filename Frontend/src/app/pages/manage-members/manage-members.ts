import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../../services/member';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-manage-members',
  templateUrl: './manage-members.html',
  styleUrls: ['./manage-members.css'],
  imports: [NgFor, NgIf, NgClass, FormsModule, DatePipe],
})
export class ManageMembersComponent implements OnInit {
  projectId!: string;
  members: any[] = [];
  loading = true;

  // invite form
  inviteEmail = '';
  inviteRole = 'Member';
  inviting = false;

  constructor(private route: ActivatedRoute, private memberService: MemberService) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.params['projectId'];
    this.loadMembers();
  }

  loadMembers() {
    this.loading = true;
    this.memberService.getMembers(this.projectId).subscribe({
      next: (res) => {
        this.members = res.data;
        this.loading = false;
      },
      error: (err) => {
        console.error(err);
        this.loading = false;
      },
    });
  }

  invite() {
    if (!this.inviteEmail) return;

    this.inviting = true;
    this.memberService.inviteMember(this.projectId, this.inviteEmail, this.inviteRole).subscribe({
      next: () => {
        alert('Invitation Sent');
        this.inviteEmail = '';
        this.inviteRole = 'Member';
        this.inviting = false;
      },
      error: (err) => {
        console.error(err);
        alert(err.error?.message || 'Error');
        this.inviting = false;
      },
    });
  }

  removeMember(userId: string) {
    if (!confirm('Remove this member?')) return;

    this.memberService.removeMember(this.projectId, userId).subscribe({
      next: () => {
        this.members = this.members.filter((m) => m.userId !== userId);
        alert('Member removed');
      },
      error: (err) => {
        alert(err.error?.message || 'Error removing member');
      },
    });
  }
}
