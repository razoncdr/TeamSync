import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TaskService } from '../../services/task';
import { ProjectService } from '../../services/project';

@Component({
  selector: 'app-project-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './project-dashboard.html',
  styleUrl: './project-dashboard.css',
})
export class ProjectDashboard implements OnInit {
  projectId!: string;
  projectName = '';
  tasks: any[] = [];
  projectMembers: any[] = [];

  showModal = false;
  editingTask: any = null;

  taskForm = {
    title: '',
    description: '',
    dueDate: '',
    status: 'Todo',
    assignedMemberIds: [] as string[],
  };

  constructor(
    private route: ActivatedRoute,
    private taskService: TaskService,
    private projectService: ProjectService
  ) {}

  ngOnInit() {
    this.projectId = this.route.snapshot.paramMap.get('id')!;
    this.loadProject();
    this.loadTasks();
  }

  loadProject() {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (res) => {
        this.projectName = res.data.name;
        this.projectMembers = res.data.members || [];
      },
      error: (err) => console.error('Failed to load project', err),
    });
  }

  loadTasks() {
    this.taskService.getTasks(this.projectId).subscribe({
      next: (res) => (this.tasks = res),
      error: (err) => console.error('Failed to load tasks', err),
    });
  }

  openModal(task: any = null) {
    this.editingTask = task;
    this.taskForm = task
      ? {
          title: task.title,
          description: task.description,
          dueDate: task.dueDate?.split('T')[0] || '',
          status: task.status,
          assignedMemberIds: task.assignedMemberIds || [],
        }
      : {
          title: '',
          description: '',
          dueDate: '',
          status: 'Todo',
          assignedMemberIds: [],
        };
    this.showModal = true;
  }

  closeModal() {
    this.showModal = false;
    this.editingTask = null;
  }

  saveTask() {
    const payload = {
      ...this.taskForm,
      projectId: this.projectId,
    };

    const action = this.editingTask
      ? this.taskService.updateTask(this.editingTask.id, payload)
      : this.taskService.createTask(payload);

    action.subscribe({
      next: () => {
        this.closeModal();
        this.loadTasks();
      },
      error: (err) => console.error('Failed to save task', err),
    });
  }

  confirmDelete(id: string) {
    if (!confirm('Are you sure you want to delete this task?')) return;
    this.taskService.deleteTask(id).subscribe({
      next: () => this.loadTasks(),
      error: (err) => console.error('Failed to delete task', err),
    });
  }
}
