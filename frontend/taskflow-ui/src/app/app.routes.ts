import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/pages/login/login.component')
        .then(m => m.LoginComponent)
  },

  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/pages/register/register.component')
        .then(m => m.RegisterComponent)
  },

  {
    path: '',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/shared/layout/dashboard-layout/dashboard-layout.component')
        .then(m => m.DashboardLayoutComponent),
    children: [
      {
        path: 'projects',
        loadComponent: () =>
          import('./features/projects/pages/project-list/project-list.component')
            .then(m => m.ProjectListComponent)
      },
      {
        path: 'projects/create',
        loadComponent: () =>
          import('./features/projects/pages/create-project/create-project.component')
            .then(m => m.CreateProjectComponent)
      },
      {
        path: 'projects/edit/:id',
        loadComponent: () =>
          import('./features/projects/pages/edit-project/edit-project.component')
            .then(m => m.EditProjectComponent)
      },
      {
        path: 'projects/:projectId/boards',
        loadComponent: () =>
          import('./features/boards/pages/board-list/board-list.component')
            .then(m => m.BoardListComponent)
      },
      {
        path: 'projects/:projectId/members',
        loadComponent: () =>
          import('./features/projects/pages/project-members/project-members.component')
            .then(m => m.ProjectMembersComponent)
      },
      {
        path: 'boards/:boardId',
        loadComponent: () =>
          import('./features/boards/pages/board-details/board-details.component')
            .then(m => m.BoardDetailsComponent)
      },
      {
        path: 'boards/:boardId/projects/:projectId/columns/:columnId/tasks/create',
        loadComponent: () =>
          import('./features/tasks/pages/create-task/create-task.component')
            .then(m => m.CreateTaskComponent)
      },
      {
        path: 'tasks/my',
        loadComponent: () =>
          import('./features/tasks/pages/my-tasks/my-tasks.component')
            .then(m => m.MyTasksComponent)
      },
      {
        path: 'tasks/:taskId/edit',
        loadComponent: () =>
          import('./features/tasks/pages/edit-task/edit-task.component')
            .then(m => m.EditTaskComponent)
      },
      {
        path: 'tasks/:taskId',
        loadComponent: () =>
          import('./features/tasks/pages/task-details/task-details.component')
            .then(m => m.TaskDetailsComponent)
      },
      {
        path: 'profile',
        loadComponent: () =>
          import('./features/profile/pages/profile/profile.component')
            .then(m => m.ProfileComponent)
      }
    ]
  }
];