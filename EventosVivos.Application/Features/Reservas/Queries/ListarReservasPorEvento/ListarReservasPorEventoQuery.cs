using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Queries.ListarReservasPorEvento;

public record ListarReservasPorEventoQuery(Guid EventoId) : IRequest<IEnumerable<ReservaDto>>;