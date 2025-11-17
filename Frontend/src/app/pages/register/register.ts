import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, NgIf } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

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

  constructor(private authService: AuthService, private router: Router) {}
  onSubmit(form: any) {
  if (form.invalid) {
    alert("Please correct the errors.");
    return;
  }

  const dto = {
    name: this.name,
    email: this.email,
    password: this.password,
  };

  this.authService.register(dto).subscribe({
    next: (res) => {
      this.router.navigate(['/login']);
    },
    error: (err) => {
      alert(err.error?.message || "Registration failed.");
    }
  });
}
}
