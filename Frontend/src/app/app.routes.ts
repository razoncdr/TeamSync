import { Routes, CanActivate } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { ProjectDashboard } from './pages/project-dashboard/project-dashboard';
import { Home } from './pages/home/home';
import { ManageMembersComponent } from './pages/manage-members/manage-members';
import { MyInvitations } from './pages/my-invitations/my-invitations';
import { ChatComponent } from './pages/chat/chat';
import { ProjectMemberGuard } from './guards/project-member-guard';
import { Forbidden } from './pages/forbidden/forbidden';
import { GuestGuard } from './guards/guest.guard';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home, canActivate: [GuestGuard] },
  { path: 'login', component: Login, canActivate: [GuestGuard]  },
  { path: 'register', component: Register, canActivate: [GuestGuard]  },
  { path: 'dashboard', component: Dashboard, canActivate: [AuthGuard]},
  { path: 'projects/:projectId', component: ProjectDashboard, canActivate: [AuthGuard, ProjectMemberGuard] }, 
  { path: 'projects/:projectId/members', component: ManageMembersComponent, canActivate: [AuthGuard, ProjectMemberGuard] },
  { path: 'projects/:projectId/chat', component: ChatComponent, canActivate: [AuthGuard, ProjectMemberGuard] },
  { path: 'my-invitations', component: MyInvitations, canActivate: [AuthGuard] },
  {path: 'forbidden', component: Forbidden},
];
