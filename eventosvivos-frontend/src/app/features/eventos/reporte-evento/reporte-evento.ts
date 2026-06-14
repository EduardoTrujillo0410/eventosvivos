import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { EventoService } from '../../../core/services/evento.service';
import { ReporteOcupacion } from '../../../core/models/evento.model';
import { DecimalPipe } from '@angular/common';

@Component({
  selector: 'app-reporte-evento',
  imports: [DecimalPipe, RouterLink],
  templateUrl: './reporte-evento.html',
  styleUrl: './reporte-evento.css',
})
export class ReporteEvento {
  private route = inject(ActivatedRoute);
  private eventoService = inject(EventoService);

  reporte = signal<ReporteOcupacion | null>(null);
  cargando = signal(true);

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.eventoService.reporte(id).subscribe({
      next: r => { this.reporte.set(r); this.cargando.set(false); },
      error: () => this.cargando.set(false)
    });
  }
}
