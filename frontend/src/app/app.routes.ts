import { Routes } from '@angular/router';
import { authGuard } from './guards/auth.guard';
import { LoginComponent } from './components/auth/login.component';
import { LayoutComponent } from './components/layout/layout.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  {
    path: '',
    component: LayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: '/livros', pathMatch: 'full' },
      { path: 'success', loadComponent: () => import('./components/shared/success.component').then(m => m.SuccessComponent) },
      { path: 'livros', loadComponent: () => import('./components/livros/livros-list.component').then(m => m.LivrosListComponent) },
      { path: 'livros/novo', loadComponent: () => import('./components/livros/livros-form.component').then(m => m.LivrosFormComponent) },
      { path: 'livros/editar/:id', loadComponent: () => import('./components/livros/livros-form.component').then(m => m.LivrosFormComponent) },
      { path: 'autores', loadComponent: () => import('./components/autores/autores-list.component').then(m => m.AutoresListComponent) },
      { path: 'autores/novo', loadComponent: () => import('./components/autores/autores-form.component').then(m => m.AutoresFormComponent) },
      { path: 'autores/editar/:id', loadComponent: () => import('./components/autores/autores-form.component').then(m => m.AutoresFormComponent) },
      { path: 'assuntos', loadComponent: () => import('./components/assuntos/assuntos-list.component').then(m => m.AssuntosListComponent) },
      { path: 'assuntos/novo', loadComponent: () => import('./components/assuntos/assuntos-form.component').then(m => m.AssuntosFormComponent) },
      { path: 'assuntos/editar/:id', loadComponent: () => import('./components/assuntos/assuntos-form.component').then(m => m.AssuntosFormComponent) },
      { path: 'formas-compra', loadComponent: () => import('./components/formas-compra/formas-compra-list.component').then(m => m.FormasCompraListComponent) },
      { path: 'formas-compra/novo', loadComponent: () => import('./components/formas-compra/formas-compra-form.component').then(m => m.FormasCompraFormComponent) },
      { path: 'formas-compra/editar/:id', loadComponent: () => import('./components/formas-compra/formas-compra-form.component').then(m => m.FormasCompraFormComponent) },
      { path: 'relatorios', loadComponent: () => import('./components/reports/reports.component').then(m => m.ReportsComponent) }
    ]
  },
  { path: '**', redirectTo: '/livros' }
];
