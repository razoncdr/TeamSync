import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, map, catchError, of } from 'rxjs';
import { ProjectService } from '../services/project';
import { ToastrService } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ProjectMemberGuard implements CanActivate {

  constructor(
    private projectService: ProjectService,
    private router: Router, 
    private toastr: ToastrService
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const projectId = route.params['projectId'];

    return this.projectService.isMember(projectId).pipe(
      map(res => {
        const isMember = res.data;

        if (!isMember) {
          this.router.navigate(['/forbidden']);
          return false;
        }

        return true;
      }),
      catchError(err => {
        console.error('Membership check failed', err);
        this.toastr.error('Failed to verify project membership', 'Error');
        this.router.navigate(['/error']);
        return of(false);
      })
    );
  }
}
