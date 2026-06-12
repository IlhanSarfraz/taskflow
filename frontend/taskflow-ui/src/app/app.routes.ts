import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'projects',
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
  }
];