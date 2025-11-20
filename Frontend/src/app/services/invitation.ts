import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}

@Injectable({ providedIn: 'root' })
export class InvitationService {
  private apiUrl = 'https://localhost:7035/api';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` }),
    };
  }

  /** -----------------------------
   * USER-LEVEL INVITATIONS
   * User sees their own invitations
   * ----------------------------- */

  getMyInvitations(): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(
      `${this.apiUrl}/invitations`,
      this.getAuthHeaders()
    );
  }

  acceptInvitation(id: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl}/invitations/${id}/accept`,
      {},
      this.getAuthHeaders()
    );
  }

  rejectInvitation(id: string): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl}/invitations/${id}/reject`,
      {},
      this.getAuthHeaders()
    );
  }

  /** -----------------------------
   * PROJECT-LEVEL INVITATIONS
   * Only Admin/Owner can access
   * ----------------------------- */

  getProjectInvitations(projectId: string): Observable<ApiResponse<any[]>> {
    return this.http.get<ApiResponse<any[]>>(
      `${this.apiUrl}/invitations/Project/${projectId}`,
      this.getAuthHeaders()
    );
  }

  cancelInvitation(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(
      `${this.apiUrl}/invitations/${id}`,
      this.getAuthHeaders()
    );
  }
}
