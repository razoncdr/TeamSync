import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { Register } from './pages/register/register';
import { Dashboard } from './pages/dashboard/dashboard';
import { ProjectDashboard } from './pages/project-dashboard/project-dashboard';
import { Home } from './pages/home/home';
import { ManageMembersComponent } from './pages/manage-members/manage-members';
import { MyInvitations } from './pages/my-invitations/my-invitations';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'register', component: Register },
  { path: 'dashboard', component: Dashboard },
  { path: 'project/:id', component: ProjectDashboard }, 
  { path: 'projects/:projectId/members', component: ManageMembersComponent },
  { path: 'my-invitations', component: MyInvitations },
];
