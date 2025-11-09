import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  email = '';
  password = '';
  constructor(private authService: AuthService) {}

  onSubmit() {
    const dto = { email: this.email, password: this.password };
    this.authService.login(dto).subscribe({
      next: (res: any) => {
        console.log('Login successful:', res);
        localStorage.setItem('token', res.token); // store JWT
        alert('Login successful!');
      },
      error: (err) => {
        console.error(err);
        alert('Login failed.');
      }
    });
  }
}
