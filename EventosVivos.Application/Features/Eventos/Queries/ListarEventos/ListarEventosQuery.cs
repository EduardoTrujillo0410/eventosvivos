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
    int Pagina = 1,      
    int Tamano = 10      
) : IRequest<PagedResultDto<EventoDto>>;