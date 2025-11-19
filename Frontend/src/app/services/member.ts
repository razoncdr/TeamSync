import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class MemberService {
  private apiUrl = 'https://localhost:7035/api';

  constructor(private http: HttpClient) {}
  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` }),
    };
  }

  getMembers(projectId: string): Observable<any> {
    return this.http.get(`${this.apiUrl}/projects/${projectId}/members`, this.getAuthHeaders());
  }

  inviteMember(projectId: string, email: string, role: string): Observable<any> {
    return this.http.post(
      `${this.apiUrl}/projects/${projectId}/members/invite`,
      {
        email,
        role,
      },
      this.getAuthHeaders()
    );
  }

  removeMember(projectId: string, userId: string): Observable<any> {
    return this.http.delete(
      `${this.apiUrl}/projects/${projectId}/members/${userId}`,
      this.getAuthHeaders()
    );
  }

  // Invitations for current logged-in user
  getMyInvitations(): Observable<any> {
    return this.http.get(`${this.apiUrl}/invitations`, this.getAuthHeaders());
  }

  acceptInvitation(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/invitations/${id}/accept`, {}, this.getAuthHeaders());
  }

  rejectInvitation(id: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/invitations/${id}/reject`, {}, this.getAuthHeaders());
  }
}
