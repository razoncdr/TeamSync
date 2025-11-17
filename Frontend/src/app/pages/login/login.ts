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
  loading = false;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router,
    private authState: AuthState
  ) {}

  onSubmit(form: NgForm) {
    // Prevent accidental submission
    if (form.invalid || this.loading) {
      return;
    }

    this.loading = true;
    this.errorMessage = '';

    const dto = { email: this.email, password: this.password };

    this.authService.login(dto).subscribe({
      next: (res: any) => {
        this.authState.login(res.token); // updates signal + stores token
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = err.error?.message || 'Invalid email or password';
        this.loading = false;
      }
    });
  }
}
