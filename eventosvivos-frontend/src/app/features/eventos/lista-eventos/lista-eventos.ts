import { Component, computed, inject, signal } from '@angular/core';
import { Evento, FiltrosEvento, PagedResultDto } from '../../../core/models/evento.model';
import { EventoService } from '../../../core/services/evento.service';
import { VenueService } from '../../../core/services/venue.service';
import { Venue } from '../../../core/models/venue.model';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-lista-eventos',
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './lista-eventos.html',
  styleUrl: './lista-eventos.css',
})
export class ListaEventos {
  private eventoService = inject(EventoService);
  private venueService = inject(VenueService);

  eventos = signal<PagedResultDto<Evento> | null>(null);
  venues = signal<Venue[]>([]);
  cargando = signal(false);
  error = signal<string | null>(null);

  filtros: FiltrosEvento = {estado: '', tipo: ''};

  private authService = inject(AuthService);
  private router = inject(Router);
  esAdmin = computed(() => this.authService.esAdmin());

  ngOnInit() {
    this.cargarVenues();
    this.cargarEventos();
  }

  salir() {
    this.authService.salir();
    this.router.navigate(['/']);
  }

  cargarVenues() {
    this.venueService.listar().subscribe(v => this.venues.set(v));
  }

  cargarEventos() {
    this.cargando.set(true);
    this.error.set(null);
    this.eventoService.listar(this.filtros).subscribe({
      next: data => { this.eventos.set(data); this.cargando.set(false); },
      error: () => { this.error.set('Error cargando eventos'); this.cargando.set(false); }
    });
  }

  aplicarFiltros() { this.cargarEventos(); }
  limpiarFiltros() { this.filtros = {}; this.cargarEventos(); }

  badgeClase(estado: string): string {
    return {
      activo: 'bg-success',
      cancelado: 'bg-danger',
      completado: 'bg-secondary'
    }[estado] ?? 'bg-secondary';
  }
}
