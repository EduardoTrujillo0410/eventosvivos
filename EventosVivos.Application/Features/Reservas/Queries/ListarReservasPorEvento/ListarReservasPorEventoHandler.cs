using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Reservas.Commands.CrearReserva;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Queries.ListarReservasPorEvento;

public class ListarReservasPorEventoHandler
    : IRequestHandler<ListarReservasPorEventoQuery, IEnumerable<ReservaDto>>
{
    private readonly IReservaRepository _reservas;

    public ListarReservasPorEventoHandler(IReservaRepository reservas)
        => _reservas = reservas;

    public async Task<IEnumerable<ReservaDto>> Handle(
        ListarReservasPorEventoQuery request, CancellationToken ct)
    {
        var reservas = await _reservas.GetByEventoIdAsync(request.EventoId, ct);
        return reservas.Select(CrearReservaHandler.MapToDto);
    }
}