import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: '',
    data: {
      title: 'System'
    },
    children: [
      {
        path: '',
        redirectTo: 'role',
        pathMatch: 'full'
      },
      {
        path: 'role',
        loadComponent: () => import('./roles/role.component').then(m => m.RoleComponent),
        data: {
          title: 'Role'
        }
      },
      {
        path: '',
        redirectTo: 'user',
        pathMatch: 'full'
      },
      {
        path: 'user',
        loadComponent: () => import('./users/user.component').then(m => m.UserComponent),
        data: {
          title: 'User'
        }
      },
    ]
  }
];


