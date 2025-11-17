import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectService } from '../../services/project';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  projects: any[] = [];
  showModal = false;
  editingProject: any = null;

  projectForm = { name: '', description: '' };

  constructor(private projectService: ProjectService, private router: Router) {}

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.projectService.getProjects().subscribe({
      next: (res) => (this.projects = res),
      error: (err) => console.error('Failed to load projects', err),
    });
  }

  goToProject(id: string) {
  this.router.navigate(['/project', id]);
}

  openModal(project: any = null) {
    this.editingProject = project;
    this.projectForm = project
      ? { name: project.name, description: project.description }
      : { name: '', description: '' };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.editingProject = null;
  }

  saveProject() {
    if (!this.projectForm.name.trim()) return;

    const action = this.editingProject
      ? this.projectService.updateProject(this.editingProject.id, this.projectForm)
      : this.projectService.createProject(this.projectForm);

    action.subscribe({
      next: () => {
        this.loadProjects();
        this.closeModal();
      },
      error: (err) => console.error('Failed to save project', err),
    });
  }

  confirmDelete(id: string) {
    if (!confirm('Are you sure you want to delete this project?')) return;
    this.projectService.deleteProject(id).subscribe({
      next: () => this.loadProjects(),
      error: (err) => console.error('Failed to delete project', err),
    });
  }

  manageMembers(id: string) {
    alert(`Members management for project ${id} will be added later`);
  }
}
