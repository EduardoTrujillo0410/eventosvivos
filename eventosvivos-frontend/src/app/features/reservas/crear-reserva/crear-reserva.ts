import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ReservaService } from '../../../core/services/reserva.service';
import { EventoService } from '../../../core/services/evento.service';
import { Evento } from '../../../core/models/evento.model';
import { Reserva } from '../../../core/models/reserva.model';

@Component({
  selector: 'app-crear-reserva',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink], // ← los 3 necesarios
  templateUrl: './crear-reserva.html',
  styleUrl: './crear-reserva.css',
})
export class CrearReserva implements OnInit {
  private fb = inject(FormBuilder);
  private reservaService = inject(ReservaService);
  private eventoService = inject(EventoService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  evento = signal<Evento | null>(null);
  reservaCreada = signal<Reserva | null>(null);
  enviando = signal(false);
  errorMsg = signal<string | null>(null);

  form = this.fb.group({
    cantidad: [1, [Validators.required, Validators.min(1)]],
    nombreComprador: ['', Validators.required],
    emailComprador: ['', [Validators.required, Validators.email]]
  });

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.eventoService.listar().subscribe(eventos => {
      const e = eventos.items.find(ev => ev.id === id);
      if (e) this.evento.set(e);
    });
  }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.enviando.set(true);
    this.errorMsg.set(null);

    const val = this.form.value;
    this.reservaService.crear({
      eventoId: this.evento()!.id,
      cantidad: val.cantidad!,
      nombreComprador: val.nombreComprador!,
      emailComprador: val.emailComprador!
    }).subscribe({
      next: r => { this.reservaCreada.set(r); this.enviando.set(false); },
      error: err => {
        this.errorMsg.set(err.error?.error ?? 'Error al crear la reserva');
        this.enviando.set(false);
      }
    });
  }

  confirmarPago(id: string) {
    this.reservaService.confirmarPago(id).subscribe({
      next: r => this.reservaCreada.set(r),
      error: err => this.errorMsg.set(err.error?.error ?? 'Error confirmando pago')
    });
  }

  cancelarReserva(id: string) {
    this.reservaService.cancelar(id).subscribe({
      next: () => this.router.navigate(['/eventos']),
      error: err => this.errorMsg.set(err.error?.error ?? 'Error cancelando')
    });
  }

  isInvalid(campo: string) {
    const c = this.form.get(campo);
    return c?.invalid && c?.touched;
  }
}