import { Component, inject } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-seleccion-rol',
  standalone: true,
  template: `
    <div class="min-vh-100 d-flex align-items-center justify-content-center bg-light">
      <div class="text-center">
        <h1 class="display-5 fw-bold mb-2">EventosVivos</h1>
        <p class="text-muted mb-5">¿Cómo deseas ingresar?</p>
        <div class="d-flex gap-4 justify-content-center">
          <button class="btn btn-primary btn-lg px-5 py-3"
            (click)="entrar('Administrador')">
             Administrador
            <div class="small fw-normal mt-1">Gestionar eventos, reservas y reportes</div>
          </button>
          <button class="btn btn-outline-primary btn-lg px-5 py-3"
            (click)="entrar('Usuario')">
             Cliente
            <div class="small fw-normal mt-1">Ver eventos y reservar</div>
          </button>
        </div>
      </div>
    </div>
  `
})
export class SeleccionRol {
  private auth = inject(AuthService);
  private router = inject(Router);

  entrar(rol: 'Administrador' | 'Usuario') {
    this.auth.seleccionarRol(rol);
    this.router.navigate(['/eventos']);
  }
}
