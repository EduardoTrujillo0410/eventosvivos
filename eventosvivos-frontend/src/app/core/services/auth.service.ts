import { Injectable, signal } from '@angular/core';

export type Rol = 'Administrador' | 'Usuario' | null;

@Injectable({ providedIn: 'root' })
export class AuthService {
  rol = signal<Rol>(null);

  seleccionarRol(rol: Rol) {
    this.rol.set(rol);
  }

  esAdmin(): boolean {
    return this.rol() === 'Administrador';
  }

  estaAutenticado(): boolean {
    return this.rol() !== null;
  }

  salir() {
    this.rol.set(null);
  }
}