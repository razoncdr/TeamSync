import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
}

@Injectable({ providedIn: 'root' })
export class TaskService {
  private apiUrl = (projectId: string) => `https://localhost:7035/api/projects/${projectId}/tasks`;

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` }),
    };
  }

  getTasks(projectId: string): Observable<ApiResponse<any[]>> {
    // GET https://localhost:7035/api/projects/{projectId}/tasks
    return this.http.get<ApiResponse<any[]>>(
      `${this.apiUrl(projectId)}`,
      this.getAuthHeaders()
    );
  }

  createTask(projectId: string, data: any): Observable<ApiResponse<any>> {
    // POST https://localhost:7035/api/projects/{projectId}/tasks
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl(projectId)}`,
      data,
      this.getAuthHeaders()
    );
  }

  updateTask(projectId: string, id: string, data: any): Observable<ApiResponse<any>> {
    // PUT https://localhost:7035/api/projects/{projectId}/tasks/{id}
    return this.http.put<ApiResponse<any>>(
      `${this.apiUrl(projectId)}/${id}`,
      data,
      this.getAuthHeaders()
    );
  }

  deleteTask(projectId: string, id: string): Observable<ApiResponse<any>> {
    // DELETE https://localhost:7035/api/projects/{projectId}/tasks/{id}
    return this.http.delete<ApiResponse<any>>(
      `${this.apiUrl(projectId)}/${id}`,
      this.getAuthHeaders()
    );
  }
}
