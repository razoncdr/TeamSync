import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { AuthState } from '../../services/auth-state';

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
  constructor(private authService: AuthService, private router: Router, private authState: AuthState) {}

  onSubmit() {
    const dto = { email: this.email, password: this.password };
    this.authService.login(dto).subscribe({
      next: (res: any) => {
        this.authState.login(res.token);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error(err);
        alert('Login failed.');
      }
    });
  }
}
