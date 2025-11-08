import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { routes } from './app/app.routes';
import { Component } from '@angular/core';
import { RouterOutlet, RouterLink } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink],
  templateUrl: './app/app.html',
  styleUrls: ['./app/app.css']
})
class AppRoot {}

bootstrapApplication(AppRoot, {
  providers: [provideRouter(routes)]
}).catch(err => console.error(err));
