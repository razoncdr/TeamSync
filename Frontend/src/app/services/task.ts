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
  private apiUrl = 'https://localhost:7035/api/Task';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ Authorization: `Bearer ${token}` }),
    };
  }

  getTasks(projectId: string): Observable<ApiResponse<any[]>> {
    console.log(`${this.apiUrl}?projectId=${projectId}`);
    return this.http.get<ApiResponse<any[]>>(
      `${this.apiUrl}?projectId=${projectId}`,
      this.getAuthHeaders()
    );
  }

  createTask(projectId: string, data: any): Observable<ApiResponse<any>> {
    return this.http.post<ApiResponse<any>>(
      `${this.apiUrl}/${projectId}`,
      data,
      this.getAuthHeaders()
    );
  }

  updateTask(id: string, data: any): Observable<ApiResponse<any>> {
    return this.http.put<ApiResponse<any>>(`${this.apiUrl}/${id}`, data, this.getAuthHeaders());
  }

  deleteTask(id: string): Observable<ApiResponse<any>> {
    return this.http.delete<ApiResponse<any>>(`${this.apiUrl}/${id}`, this.getAuthHeaders());
  }
}
