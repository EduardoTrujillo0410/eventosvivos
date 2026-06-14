using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Commands.CrearEvento;

public record CrearEventoCommand(
    string Titulo,
    string Descripcion,
    int VenueId,
    int CapacidadMaxima,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal PrecioEntrada,
    string Tipo
) : IRequest<EventoDto>;