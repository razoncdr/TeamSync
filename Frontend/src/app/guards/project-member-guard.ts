import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, Router } from '@angular/router';
import { Observable, map, catchError, of } from 'rxjs';
import { ProjectService } from '../services/project';

@Injectable({
  providedIn: 'root'
})
export class ProjectMemberGuard implements CanActivate {

  constructor(
    private projectService: ProjectService,
    private router: Router
  ) {}

  canActivate(route: ActivatedRouteSnapshot): Observable<boolean> {
    const projectId = route.params['projectId'];
    console.log("Project Id in project member guard: ", projectId);

    return this.projectService.isMember(projectId).pipe(
      map(res => {
        const isMember = res.data;

        console.log('ProjectMemberGuard: isMember =', isMember);

        if (!isMember) {
          this.router.navigate(['/forbidden']);
          return false;
        }

        return true;
      }),
      catchError(err => {
        console.error('Membership check failed', err);
        this.router.navigate(['/error']);
        return of(false);
      })
    );
  }
}
