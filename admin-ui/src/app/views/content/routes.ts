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
      title: 'Content'
    },
    children: [
      {
        path: '',
        redirectTo: 'post',
        pathMatch: 'full'
      },
      {
        path: 'post',
        loadComponent: () => import('./post/post.component').then(m => m.PostComponent),
        data: {
          title: 'Post'
        }
      },
    ]
  }
];


