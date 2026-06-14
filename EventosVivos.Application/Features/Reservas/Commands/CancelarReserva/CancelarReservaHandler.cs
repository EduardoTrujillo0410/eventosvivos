using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Reservas.Commands.CrearReserva;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.CancelarReserva;

public class CancelarReservaHandler : IRequestHandler<CancelarReservaCommand, ReservaDto>
{
    private readonly IReservaRepository _reservas;

    public CancelarReservaHandler(IReservaRepository reservas) => _reservas = reservas;

    public async Task<ReservaDto> Handle(CancelarReservaCommand request, CancellationToken ct)
    {
        var reserva = await _reservas.GetByIdAsync(request.ReservaId, ct)
            ?? throw new DomainException($"Reserva {request.ReservaId} no encontrada.");

        if (reserva.Estado == EstadoReserva.Confirmada)
            reserva.CancelarConfirmada(reserva.Evento.FechaInicio);
        else
            reserva.Cancelar(reserva.Evento.FechaInicio);

        await _reservas.SaveChangesAsync(ct);

        return CrearReservaHandler.MapToDto(reserva);
    }
}