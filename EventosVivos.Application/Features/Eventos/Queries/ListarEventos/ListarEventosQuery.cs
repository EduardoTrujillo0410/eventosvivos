using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Queries.ListarEventos;

public record ListarEventosQuery(
    string? Tipo,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int? VenueId,
    string? Estado,
    string? Titulo,
    int Pagina = 1,      // defecto página 1
    int Tamano = 10      // defecto 10 por página
) : IRequest<PagedResultDto<EventoDto>>;