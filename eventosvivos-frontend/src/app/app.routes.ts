import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/auth/seleccion-rol/seleccion-rol')
      .then(m => m.SeleccionRol)
  },
  {
    path: 'eventos',
    canActivate: [authGuard],
    loadComponent: () => import('./features/eventos/lista-eventos/lista-eventos')
      .then(m => m.ListaEventos)
  },
  {
    path: 'eventos/crear',
    canActivate: [authGuard],
    loadComponent: () => import('./features/eventos/crear-evento/crear-evento')
      .then(m => m.CrearEvento)
  },
  {
    path: 'eventos/:id/reporte',
    canActivate: [authGuard],
    loadComponent: () => import('./features/eventos/reporte-evento/reporte-evento')
      .then(m => m.ReporteEvento)
  },
  {
    path: 'eventos/:id/reservar',
    canActivate: [authGuard],
    loadComponent: () => import('./features/reservas/crear-reserva/crear-reserva')
      .then(m => m.CrearReserva)
  },
  {
    path: 'eventos/:id/reservas',
    canActivate: [authGuard],
    loadComponent: () => import('./features/reservas/gestion-reservas/gestion-reservas')
      .then(m => m.GestionReservas)
  },
  {
    path: '**',
    redirectTo: ''
  }
];