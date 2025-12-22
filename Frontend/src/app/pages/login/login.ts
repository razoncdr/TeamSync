import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { CommonModule, NgIf } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule, NgIf],
  templateUrl: './login.html',
  styleUrls: ['./login.css'],
})
export class Login {
  email = '';
  password = '';

  showPassword = false;
  submitted = false;
  isLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private auth: AuthService, 
    private toastr: ToastrService
  ) {}

  onSubmit(form: NgForm) {
    this.submitted = true;
    this.isLoading = true;

    if (form.invalid) return;

    const dto = { email: this.email, password: this.password };

    this.authService.login(dto).subscribe({
      next: (res: any) => {
        localStorage.setItem('token', res.body.token);
        this.router.navigate(['/dashboard']);
        this.isLoading = false;
      },
      error: (err) => {
        this.toastr.error('Invalid credentials.', 'Error');
        this.isLoading = false;
      },
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }
}
