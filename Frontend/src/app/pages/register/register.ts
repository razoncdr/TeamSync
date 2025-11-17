import { Component } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';
import { CommonModule, NgIf } from '@angular/common';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink, CommonModule, NgIf],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
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

  constructor(private authService: AuthService, private router: Router) {}

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
      password: this.password 
    };

    this.authService.register(dto).subscribe({
      next: (res) => {
        console.log('Registered user:', res);
        alert('Registration successful!');
        this.router.navigate(['/login']);
      },
      error: (err) => {
        console.error(err);
        alert('Registration failed.');
      }
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
}
