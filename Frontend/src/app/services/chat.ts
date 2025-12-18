import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface ApiResponse<T> {
  success: boolean;
  data: T;
}

export interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  message: string;
  createdAt: string;
}

@Injectable({
  providedIn: 'root',
})
export class ChatService {
  private readonly baseUrl = 'https://localhost:7035/api/projects';

  constructor(private http: HttpClient) {}

  private getAuthHeaders() {
    const token = localStorage.getItem('token');
    return {
      headers: new HttpHeaders({
        Authorization: `Bearer ${token}`,
      }),
    };
  }

  // GET: /api/projects/{projectId}/chat
  getChatMessages(
  projectId: string,
  skip = 0,
  limit = 20
): Observable<ApiResponse<ChatMessage[]>> {
  return this.http.get<ApiResponse<ChatMessage[]>>(
    `${this.baseUrl}/${projectId}/chat?skip=${skip}&limit=${limit}`,
    this.getAuthHeaders()
  );
}


  // POST: /api/projects/{projectId}/chat
  sendMessage(
    projectId: string,
    message: string
  ): Observable<ApiResponse<ChatMessage>> {
    return this.http.post<ApiResponse<ChatMessage>>(
      `${this.baseUrl}/${projectId}/chat`,
      { message },
      this.getAuthHeaders()
    );
  }
}
