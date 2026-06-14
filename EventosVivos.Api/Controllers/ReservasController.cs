using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Reservas.Commands.CancelarReserva;
using EventosVivos.Application.Features.Reservas.Commands.ConfirmarPago;
using EventosVivos.Application.Features.Reservas.Commands.CrearReserva;
using EventosVivos.Application.Features.Reservas.Queries.ListarReservasPorEvento;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventosVivos.API.Controllers;

[EnableRateLimiting("reservas")]
[ApiController]
[Route("api/[controller]")]
public class ReservasController : ControllerBase
{
    private readonly IMediator _mediator;
    public ReservasController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(ReservaDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear([FromBody] CrearReservaRequest request)
    {
        var command = new CrearReservaCommand(
            request.EventoId, request.Cantidad,
            request.NombreComprador, request.EmailComprador);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Crear), new { id = result.Id }, result);
    }

    [HttpPatch("{id:guid}/confirmar-pago")]
    [ProducesResponseType(typeof(ReservaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ConfirmarPago(Guid id)
    {
        var result = await _mediator.Send(new ConfirmarPagoCommand(id));
        return Ok(result);
    }

    [HttpPatch("{id:guid}/cancelar")]
    [ProducesResponseType(typeof(ReservaDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Cancelar(Guid id)
    {
        var result = await _mediator.Send(new CancelarReservaCommand(id));
        return Ok(result);
    }

    [HttpGet("evento/{eventoId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<ReservaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListarPorEvento(Guid eventoId)
    {
        var result = await _mediator.Send(new ListarReservasPorEventoQuery(eventoId));
        return Ok(result);
    }
}