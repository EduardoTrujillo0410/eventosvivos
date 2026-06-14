export type EstadoReserva = 'PendientePago' | 'Confirmada' | 'Cancelada';

export interface Reserva {
  id: string;
  eventoId: string;
  eventoTitulo: string;
  cantidad: number;
  nombreComprador: string;
  emailComprador: string;
  estado: EstadoReserva;
  codigoReserva?: string;
  creadaEn: string;
  canceladaEn?: string;
}

export interface CrearReservaRequest {
  eventoId: string;
  cantidad: number;
  nombreComprador: string;
  emailComprador: string;
}