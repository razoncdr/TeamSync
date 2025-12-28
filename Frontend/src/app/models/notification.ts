export interface Notification {
  id: string;
  title: string;
  message: string;
  type: 'task' | 'project' | 'system';
  isRead: boolean;
  createdAt: string;
  metadata?: any; // taskId, projectId etc
}
