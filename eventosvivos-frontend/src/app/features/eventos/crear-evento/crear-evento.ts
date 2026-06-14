import { Component, inject, signal } from '@angular/core';
import { EventoService } from '../../../core/services/evento.service';
import { VenueService } from '../../../core/services/venue.service';
import { Venue } from '../../../core/models/venue.model';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-crear-evento',
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './crear-evento.html',
  styleUrl: './crear-evento.css',
})
export class CrearEvento {
  private fb = inject(FormBuilder);
  private eventoService = inject(EventoService);
  private venueService = inject(VenueService);
  private router = inject(Router);

  venues = signal<Venue[]>([]);
  enviando = signal(false);
  errorMsg = signal<string | null>(null);

  form = this.fb.group({
    titulo: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
    descripcion: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
    venueId: [null as number | null, Validators.required],
    capacidadMaxima: [null as number | null, [Validators.required, Validators.min(1)]],
    fechaInicio: ['', Validators.required],
    fechaFin: ['', Validators.required],
    precioEntrada: [null as number | null, [Validators.required, Validators.min(0.01)]],
    tipo: ['', Validators.required]
  });

  ngOnInit() {
    this.venueService.listar().subscribe(v => this.venues.set(v));
  }

  submit() {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.enviando.set(true);
    this.errorMsg.set(null);

    const val = this.form.value;
    this.eventoService.crear({
      titulo: val.titulo!,
      descripcion: val.descripcion!,
      venueId: val.venueId!,
      capacidadMaxima: val.capacidadMaxima!,
      fechaInicio: new Date(val.fechaInicio!).toISOString(),
      fechaFin: new Date(val.fechaFin!).toISOString(),
      precioEntrada: val.precioEntrada!,
      tipo: val.tipo!
    }).subscribe({
      next: () => this.router.navigate(['/eventos']),
      error: (err) => {
        this.errorMsg.set(err.error?.error ?? 'Error al crear el evento');
        this.enviando.set(false);
      }
    });
  }

  isInvalid(campo: string) {
    const c = this.form.get(campo);
    return c?.invalid && c?.touched;
  }
}
