import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { TaskService } from '../../services/task';
import { ProjectService } from '../../services/project';
import { ToastrService } from 'ngx-toastr';
import { MemberService } from '../../services/member';

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
    private projectService: ProjectService, 
    private memberService: MemberService,
    private toastr: ToastrService
  ) {}

  ngOnInit() {
    this.projectId = this.route.snapshot.paramMap.get('projectId')!;
    this.loadProject();
    this.loadProjectMembers();
    this.loadTasks();
  }

  loadProject() {
    this.projectService.getProjectById(this.projectId).subscribe({
      next: (res) => {
        this.projectName = res.data.name;
      },
      error: (err) => this.toastr.error('Failed to load project', 'Error'),
    });
  }

  loadProjectMembers() {
    this.memberService.getMembers(this.projectId).subscribe({
      next: (res) => {
        this.projectMembers = res.data || [];
      },
      error: (err) => this.toastr.error('Failed to load project members', 'Error'),
    });
  }

  loadTasks() {
    this.taskService.getTasks(this.projectId).subscribe({
      next: (res) => (this.tasks = res.data || []),
      error: (err) => this.toastr.error('Failed to load tasks', 'Error'),
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

  saveTask(form: NgForm) {
    if (form.invalid) {
      form.control.markAllAsTouched();
      return;
    }
    const action = this.editingTask
      ? this.taskService.updateTask(this.projectId, this.editingTask.id, this.taskForm)
      : this.taskService.createTask(this.projectId, this.taskForm);

    action.subscribe({
      next: () => {
        this.editingTask ? this.toastr.success('Task updated successfully', 'Success') : this.toastr.success('Task created successfully', 'Success');
        this.closeModal();
        this.loadTasks();
      },
      error: (err) => this.toastr.error('Failed to save task', 'Error'),
    });
  }

  confirmDelete(id: string) {
    if (!confirm('Are you sure you want to delete this task?')) return;
    this.taskService.deleteTask(this.projectId, id).subscribe({
      next: () => {
        this.toastr.success('Task deleted successfully', 'Success');
        this.loadTasks();
      },
      error: (err) => this.toastr.error('Failed to delete task', 'Error'),
    });
  }
}
