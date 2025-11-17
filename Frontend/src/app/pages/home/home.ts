// src/app/pages/home/home.ts
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink],
  template: `
    <h1>Welcome to TeamSync</h1>
    <p>Please login or register to continue.</p>
    <a routerLink="/login">Login</a> |
    <a routerLink="/register">Register</a>
  `
})
export class Home {}
