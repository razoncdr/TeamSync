import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { CommonModule, NgIf } from '@angular/common';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule, NgIf],
  templateUrl: './register.html',
  styleUrls: ['./register.css'],
})
export class Register {
  name = '';
  email = '';
  password = '';
  confirmPassword = '';

  showPassword = false;
  showConfirmPassword = false;

  // Validation flags
  submitted = false;

  constructor(private authService: AuthService, private router: Router, private toastr: ToastrService) {}

  passwordsMatch(): boolean {
    return this.password === this.confirmPassword;
  }

  onSubmit(form: NgForm) {
    this.submitted = true;

    if (form.invalid || !this.passwordsMatch()) {
      return; // Stop submit if validation fails
    }

    const dto = {
      name: this.name,
      email: this.email,
      password: this.password,
    };

    this.authService.register(dto).subscribe({
      next: (res) => {
        this.toastr.success(res.body.message, 'Success');

        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.toastr.error(err.error?.message || 'Registration failed', 'Error');
        if (err.error.message) this.toastr.error(`Registration failed: ${err.error.message}`, 'Error');
        else
            this.toastr.error(`Registration failed: ${
              err.error.errors.Name[0] || err.error.errors.Email[0] || err.error.errors.Password[0]
            }`, 'Error');
      },
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
