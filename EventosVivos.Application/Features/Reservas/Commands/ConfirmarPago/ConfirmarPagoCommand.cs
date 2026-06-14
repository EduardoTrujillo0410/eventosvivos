using EventosVivos.Application.DTOs;
using MediatR;

namespace EventosVivos.Application.Features.Reservas.Commands.ConfirmarPago;

public record ConfirmarPagoCommand(Guid ReservaId) : IRequest<ReservaDto>;