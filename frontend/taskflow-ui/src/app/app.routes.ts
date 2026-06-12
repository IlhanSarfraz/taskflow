import { Routes } from '@angular/router';

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
  }
];
