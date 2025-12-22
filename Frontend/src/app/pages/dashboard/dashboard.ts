import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ProjectService } from '../../services/project';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  projects: any[] = [];
  showModal = false;
  editingProject: any = null;

  projectForm = { name: '', description: '' };

  constructor(private projectService: ProjectService, private router: Router, private toastr: ToastrService) {}

  ngOnInit() {
    this.loadProjects();
  }

  loadProjects() {
    this.projectService.getProjects().subscribe({
      next: (res) => {this.projects = res.data || [];
      },
      error: (err) => {
        this.toastr.error('Failed to load projects', 'Error');
      }
    });
  }

  goToProject(projectId: string) {
    this.router.navigate([`/projects/${projectId}`]);
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

  saveProject(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }

    const action = this.editingProject
      ? this.projectService.updateProject(this.editingProject.id, this.projectForm)
      : this.projectService.createProject(this.projectForm);

    action.subscribe({
      next: () => {
        this.editingProject ? this.toastr.success('Project updated successfully', 'Success') : this.toastr.success('Project created successfully', 'Success');
        this.loadProjects();
        this.closeModal();
      },
      error: (err) => this.toastr.error('Failed to save project', err),
    });
  }

  confirmDelete(id: string) {
    if (!confirm('Are you sure you want to delete this project?')) return;
    this.projectService.deleteProject(id).subscribe({
      next: () => {
        this.toastr.success('Project deleted successfully', 'Success');
        this.loadProjects();
      },
      error: (err) => this.toastr.error('Failed to delete project', 'Error'),
    });
  }

  manageMembers(id: string) {
    this.router.navigate([`/projects/${id}/members`]);
  }
  manageChat(id: string) {
    this.router.navigate([`/projects/${id}/chat`]);
  }
}
