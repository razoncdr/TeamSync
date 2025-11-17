import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface RegisterDto {
  name: string;
  email: string;
  password: string;
}

interface LoginDto {
  email: string;
  password: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'https://localhost:7035/api/Auth';

  constructor(private http: HttpClient) {}

  register(dto: RegisterDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/register`, dto, { observe: 'response' });
  }

  login(dto: LoginDto): Observable<any> {
    return this.http.post(`${this.apiUrl}/login`, dto, { observe: 'response' });
  }
}
