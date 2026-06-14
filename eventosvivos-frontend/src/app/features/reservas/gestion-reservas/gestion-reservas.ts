import { CommonModule } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ReservaService } from '../../../core/services/reserva.service';
import { EventoService } from '../../../core/services/evento.service';
import { Reserva } from '../../../core/models/reserva.model';
import { Evento } from '../../../core/models/evento.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-gestion-reservas',
  imports: [CommonModule, RouterLink],
  templateUrl: './gestion-reservas.html',
  styleUrl: './gestion-reservas.css',
})
export class GestionReservas {
  private route = inject(ActivatedRoute);
  private reservaService = inject(ReservaService);
  private eventoService = inject(EventoService);

  reservas = signal<Reserva[]>([]);
  evento = signal<Evento | null>(null);
  cargando = signal(true);
  mensaje = signal<{ texto: string; tipo: string } | null>(null);

  private authService = inject(AuthService);
  esAdmin = computed(() => this.authService.esAdmin());

  ngOnInit() {
    const eventoId = this.route.snapshot.paramMap.get('id')!;
    this.cargarDatos(eventoId);
  }

  cargarDatos(eventoId: string) {
    this.eventoService.listar().subscribe(result => {
      const e = result.items.find(ev => ev.id === eventoId);
      if (e) this.evento.set(e);
    });

    this.reservaService.listarPorEvento(eventoId).subscribe({
      next: r => { this.reservas.set(r); this.cargando.set(false); },
      error: () => this.cargando.set(false)
    });
  }

  confirmarPago(reservaId: string) {
    this.reservaService.confirmarPago(reservaId).subscribe({
      next: r => {
        this.actualizarReserva(r);
        this.mostrarMensaje(`Pago confirmado. Código: ${r.codigoReserva}`, 'success');
      },
      error: err => this.mostrarMensaje(err.error?.error ?? 'Error confirmando', 'danger')
    });
  }

  cancelar(reservaId: string) {
    this.reservaService.cancelar(reservaId).subscribe({
      next: r => {
        this.actualizarReserva(r);
        this.mostrarMensaje('Reserva cancelada', 'warning');
      },
      error: err => this.mostrarMensaje(err.error?.error ?? 'Error cancelando', 'danger')
    });
  }

  private actualizarReserva(reservaActualizada: Reserva) {
    this.reservas.update(lista =>
      lista.map(r => r.id === reservaActualizada.id ? reservaActualizada : r)
    );
  }

  private mostrarMensaje(texto: string, tipo: string) {
    this.mensaje.set({ texto, tipo });
    setTimeout(() => this.mensaje.set(null), 4000);
  }

  badgeEstado(estado: string): string {
    return {
      'PendientePago': 'bg-warning text-dark',
      'Confirmada': 'bg-success',
      'Cancelada': 'bg-danger'
    }[estado] ?? 'bg-secondary';
  }
}
