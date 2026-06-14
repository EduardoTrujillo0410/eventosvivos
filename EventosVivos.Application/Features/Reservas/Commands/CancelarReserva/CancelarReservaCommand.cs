using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.CancelarReserva;

public record CancelarReservaCommand(Guid ReservaId) : IRequest<ReservaDto>;