import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../../services/member';
import { DatePipe, NgClass, NgFor, NgIf } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { InvitationService } from '../../services/invitation';
import { ProjectService } from '../../services/project';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-manage-members',
  templateUrl: './manage-members.html',
  styleUrls: ['./manage-members.css'],
  imports: [NgFor, NgIf, NgClass, FormsModule, DatePipe],
})
export class ManageMembersComponent implements OnInit {
  projectId!: string;
  projectName: string = '';
  members: any[] = [];
  loading = true;

  // invite form
  inviteEmail = '';
  inviteRole = 'Member';
  inviting = false;
  invitations: any[] = [];
  loadingInvitations = true;

  constructor(
    private route: ActivatedRoute,
    private memberService: MemberService,
    private invitationService: InvitationService,
    private projectService: ProjectService, 
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.projectId = this.route.snapshot.params['projectId'];
    this.loadProject();
    this.loadMembers();
    this.loadInvitations();
  }

  loadProject() {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (res) => {
        // get project name
        this.projectName = res.data?.name || '';
      }
    });
  }


  // load members with name and email
  loadMembers() {
    this.loading = true;
    this.memberService.getMembers(this.projectId).subscribe({
      next: (res) => {
        this.members = res.data;
        this.loading = false;
      },
      error: (err) => {
        this.toastr.error('Failed to load members', 'Error');
        this.loading = false;
      },
    });
  }

  loadInvitations() {
    this.loadingInvitations = true;
    this.invitationService.getProjectInvitations(this.projectId).subscribe({
      next: (res) => {
        this.invitations = res.data || [];
        this.loadingInvitations = false;
      },
      error: (err) => {
        this.toastr.error('Failed to load invitations', 'Error');
        this.loadingInvitations = false;
      },
    });
  }

  invite() {
    if (!this.inviteEmail) return;

    this.inviting = true;
    this.memberService.inviteMember(this.projectId, this.inviteEmail, this.inviteRole).subscribe({
      next: () => {
        this.toastr.success('Invitation sent successfully', 'Success');
        this.loadInvitations();
        this.inviteEmail = '';
        this.inviteRole = 'Member';
        this.inviting = false;
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Error sending invitation', 'Error');
        this.inviting = false;
      },
    });
  }

  cancelInvitation(invitationId: string) {
    if (!confirm('Cancel this invitation?')) return;
    this.invitationService.cancelInvitation(invitationId).subscribe({
      next: () => {
        this.loadInvitations();
        this.toastr.success("Invitation cancelled successfully", "Success");
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Error cancelling invitation', 'Error');
      },
    });
  }

  removeMember(userId: string) {
    if (!confirm('Remove this member?')) return;

    this.memberService.removeMember(this.projectId, userId).subscribe({
      next: () => {
        this.members = this.members.filter((m) => m.userId !== userId);
        this.toastr.success("Member removed successfully", "Success");
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Error removing member', 'Error');
      },
    });
  }
}
