import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProjectService } from '../../services/project';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  projects: any[] = [];
  newProjectName = '';

  constructor(private projectService: ProjectService) {}

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.projectService.getProjects().subscribe({
      next: (res) => this.projects = res,
      error: (err) => console.error('Failed to load projects', err)
    });
  }

  createProject() {
    if (!this.newProjectName) return;

    const project = { name: this.newProjectName };
    this.projectService.createProject(project).subscribe({
      next: () => {
        this.newProjectName = '';
        this.loadProjects(); // refresh
      },
      error: (err) => console.error('Failed to create project', err)
    });
  }

  deleteProject(id: string) {
    this.projectService.deleteProject(id).subscribe({
      next: () => this.loadProjects(),
      error: (err) => console.error('Failed to delete project', err)
    });
  }
}
