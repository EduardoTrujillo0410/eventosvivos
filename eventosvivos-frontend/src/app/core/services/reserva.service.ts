import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Reserva, CrearReservaRequest } from '../models/reserva.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReservaService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/reservas`;

  crear(request: CrearReservaRequest): Observable<Reserva> {
    return this.http.post<Reserva>(this.base, request);
  }

  confirmarPago(id: string): Observable<Reserva> {
    return this.http.patch<Reserva>(`${this.base}/${id}/confirmar-pago`, {});
  }

  cancelar(id: string): Observable<Reserva> {
    return this.http.patch<Reserva>(`${this.base}/${id}/cancelar`, {});
  }

  listarPorEvento(eventoId: string): Observable<Reserva[]> {
    return this.http.get<Reserva[]>(`${environment.apiUrl}/reservas/evento/${eventoId}`);
  }
}