import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { AuthState } from '../../services/auth-state';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule, NgIf],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class Login {
  email = '';
  password = '';

  showPassword = false;
  submitted = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private authState: AuthState
  ) {}

  onSubmit(form: NgForm) {
    this.submitted = true;

    if (form.invalid) return;

    const dto = { email: this.email, password: this.password };

    this.authService.login(dto).subscribe({
      next: (res: any) => {
        this.authState.login(res.token);
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error(err);
        alert('Invalid credentials.');
      }
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }
}
