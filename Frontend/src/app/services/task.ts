import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TaskService {
  private apiUrl = 'https://localhost:7035/api/Task';

  constructor(private http: HttpClient) {
    console.log('TaskService initialized');
  }

    private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({ 'Authorization': `Bearer ${token}` })
    };
  }

  getTasks(projectId: string) {
    console.log(`${this.apiUrl}?projectId=${projectId}`);
    return this.http.get<any[]>(`${this.apiUrl}?projectId=${projectId}`, this.getAuthHeaders());
  }

  createTask(data: any) {
    return this.http.post(this.apiUrl, data, this.getAuthHeaders());
  }

  updateTask(id: string, data: any) {
    return this.http.put(`${this.apiUrl}/${id}`, data, this.getAuthHeaders());
  }

  deleteTask(id: string) {
    return this.http.delete(`${this.apiUrl}/${id}`, this.getAuthHeaders());
  }
}
