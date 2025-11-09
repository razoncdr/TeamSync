import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register {
  name = '';
  email = '';
  password = '';

  constructor(private authService: AuthService) {}

  onSubmit() {
    const dto = { name: this.name, email: this.email, password: this.password };
    this.authService.register(dto).subscribe({
      next: (res) => {
        console.log('Registered user:', res);
        alert('Registration successful!');
      },
      error: (err) => {
        console.error(err);
        alert('Registration failed.');
      }
    });
  }
}
