import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },

  {
    path: 'projects',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/pages/project-list/project-list.component')
        .then(m => m.ProjectListComponent)
  },

  {
    path: 'projects/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/pages/create-project/create-project.component')
        .then(m => m.CreateProjectComponent)
  },

  {
    path: 'projects/edit/:id',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/pages/edit-project/edit-project.component')
        .then(m => m.EditProjectComponent)
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
    path: 'projects/:projectId/boards',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/boards/pages/board-list/board-list.component')
        .then(m => m.BoardListComponent)
  },
  {
    path: 'boards/:boardId',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/boards/pages/board-details/board-details.component')
        .then(m => m.BoardDetailsComponent)
  },
  {
    path: 'boards/:boardId/projects/:projectId/columns/:columnId/tasks/create',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/tasks/pages/create-task/create-task.component')
        .then(m => m.CreateTaskComponent)
  },
  {
    path: 'tasks/:taskId',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/tasks/pages/task-details/task-details.component')
        .then(m => m.TaskDetailsComponent)
  },
  {
    path: `tasks/:taskId/edit`,
    canActivate: [authGuard],
    loadComponent: () =>
      import(`./features/tasks/pages/edit-task/edit-task.component`)
          .then(m => m.EditTaskComponent)
  },
  {
    path: 'projects/:projectId/members',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/projects/pages/project-members/project-members.component')
        .then(m => m.ProjectMembersComponent)
  }
];