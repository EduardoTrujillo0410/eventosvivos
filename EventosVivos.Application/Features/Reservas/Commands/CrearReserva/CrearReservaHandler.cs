using EventosVivos.Application.DTOs;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.CrearReserva;

public class CrearReservaHandler : IRequestHandler<CrearReservaCommand, ReservaDto>
{
    private readonly IEventoRepository _eventos;
    private readonly IReservaRepository _reservas;

    public CrearReservaHandler(IEventoRepository eventos, IReservaRepository reservas)
    {
        _eventos = eventos;
        _reservas = reservas;
    }

    public async Task<ReservaDto> Handle(CrearReservaCommand request, CancellationToken ct)
    {
        var evento = await _eventos.GetByIdAsync(request.EventoId, ct)
            ?? throw new DomainException($"Evento {request.EventoId} no encontrado.");

        evento.ActualizarEstadoSegunFecha();

        if (evento.Estado != Domain.Enums.EstadoEvento.Activo)
            throw new DomainException("Solo se pueden reservar entradas para eventos activos.");

        var reserva = Reserva.Create(evento, request.Cantidad,
            request.NombreComprador, request.EmailComprador);

        await _reservas.AddAsync(reserva, ct);
        await _reservas.SaveChangesAsync(ct);

        return MapToDto(reserva);
    }

    public static ReservaDto MapToDto(Reserva r) => new(
        r.Id, r.EventoId, r.Evento.Titulo, r.Cantidad,
        r.NombreComprador, r.EmailComprador,
        r.Estado.ToString(), r.CodigoReserva,
        r.CreadaEn, r.CanceladaEn
    );
}