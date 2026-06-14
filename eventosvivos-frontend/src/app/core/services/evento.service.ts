import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Evento, CrearEventoRequest, FiltrosEvento, ReporteOcupacion, PagedResultDto } from '../models/evento.model';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EventoService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/eventos`;

  listar(filtros: FiltrosEvento = {}, pagina = 1, tamano = 10): Observable<PagedResultDto<Evento>> {
    let params = new HttpParams();
    if (filtros.tipo) params = params.set('tipo', filtros.tipo);
    if (filtros.fechaDesde) params = params.set('fechaDesde', filtros.fechaDesde);
    if (filtros.fechaHasta) params = params.set('fechaHasta', filtros.fechaHasta);
    if (filtros.venueId) params = params.set('venueId', filtros.venueId);
    if (filtros.estado) params = params.set('estado', filtros.estado);
    if (filtros.titulo) params = params.set('titulo', filtros.titulo);
    params = params.set('pagina', pagina);
    params = params.set('tamano', tamano);
    return this.http.get<PagedResultDto<Evento>>(this.base, { params });
  }

  crear(request: CrearEventoRequest): Observable<Evento> {
    return this.http.post<Evento>(this.base, request);
  }

  reporte(id: string): Observable<ReporteOcupacion> {
    return this.http.get<ReporteOcupacion>(`${this.base}/${id}/reporte`);
  }
}