using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Queries.ReporteOcupacion;

public record ReporteOcupacionQuery(Guid EventoId) : IRequest<ReporteOcupacionDto>;