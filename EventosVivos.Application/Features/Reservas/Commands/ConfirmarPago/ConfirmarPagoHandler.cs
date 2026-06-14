using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Reservas.Commands.CrearReserva;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.ConfirmarPago;

public class ConfirmarPagoHandler : IRequestHandler<ConfirmarPagoCommand, ReservaDto>
{
    private readonly IReservaRepository _reservas;

    public ConfirmarPagoHandler(IReservaRepository reservas) => _reservas = reservas;

    public async Task<ReservaDto> Handle(ConfirmarPagoCommand request, CancellationToken ct)
    {
        var reserva = await _reservas.GetByIdAsync(request.ReservaId, ct)
            ?? throw new DomainException($"Reserva {request.ReservaId} no encontrada.");

        var codigo = await GenerarCodigoUnico(ct);
        reserva.ConfirmarPago(codigo);

        await _reservas.SaveChangesAsync(ct);
        return CrearReservaHandler.MapToDto(reserva);
    }

    private async Task<string> GenerarCodigoUnico(CancellationToken ct)
    {
        string codigo;
        do
        {
            var numero = Random.Shared.Next(0, 999999).ToString("D6");
            codigo = $"EV-{numero}";
        }
        while (await _reservas.ExisteCodigoReserva(codigo, ct));

        return codigo;
    }
}