export type TipoEvento = 'conferencia' | 'taller' | 'concierto';
export type EstadoEvento = 'activo' | 'cancelado' | 'completado';

export interface Evento {
  id: string;
  titulo: string;
  descripcion: string;
  venueId: number;
  venueNombre: string;
  capacidadMaxima: number;
  entradasDisponibles: number;
  fechaInicio: string;
  fechaFin: string;
  precioEntrada: number;
  tipo: TipoEvento;
  estado: EstadoEvento;
}

export interface CrearEventoRequest {
  titulo: string;
  descripcion: string;
  venueId: number;
  capacidadMaxima: number;
  fechaInicio: string;
  fechaFin: string;
  precioEntrada: number;
  tipo: string;
}

export interface FiltrosEvento {
  tipo?: string;
  fechaDesde?: string;
  fechaHasta?: string;
  venueId?: number;
  estado?: string;
  titulo?: string;
}

export interface ReporteOcupacion {
  eventoId: string;
  eventoTitulo: string;
  entradasVendidas: number;
  entradasDisponibles: number;
  entradasPerdidas: number;
  porcentajeOcupacion: number;
  totalIngresos: number;
  estadoEvento: string;
}

export interface PagedResultDto<T> {
  items: T[];
  totalItems: number;
  pagina: number;
  tamanoPage: number;
  totalPaginas: number;
  tieneSiguiente: boolean;
  tieneAnterior: boolean;
}