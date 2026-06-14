using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.CrearReserva;

public record CrearReservaCommand(
    Guid EventoId,
    int Cantidad,
    string NombreComprador,
    string EmailComprador
) : IRequest<ReservaDto>;