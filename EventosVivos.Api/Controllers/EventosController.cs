using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Eventos.Commands.CrearEvento;
using EventosVivos.Application.Features.Eventos.Queries.ListarEventos;
using EventosVivos.Application.Features.Eventos.Queries.ReporteOcupacion;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EventosVivos.API.Controllers;

[EnableRateLimiting("general")]
[ApiController]
[Route("api/[controller]")]
public class EventosController : ControllerBase
{
    private readonly IMediator _mediator;
    public EventosController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    [ProducesResponseType(typeof(EventoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Crear([FromBody] CrearEventoRequest request)
    {
        var command = new CrearEventoCommand(
            request.Titulo, request.Descripcion, request.VenueId,
            request.CapacidadMaxima, request.FechaInicio.ToLocalTime(), request.FechaFin.ToLocalTime(),
            request.PrecioEntrada, request.Tipo);

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Reporte), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
    [FromQuery] string? tipo,
    [FromQuery] DateTime? fechaDesde,
    [FromQuery] DateTime? fechaHasta,
    [FromQuery] int? venueId,
    [FromQuery] string? estado,
    [FromQuery] string? titulo,
    [FromQuery] int pagina = 1,
    [FromQuery] int tamano = 10)
    {
        var result = await _mediator.Send(
            new ListarEventosQuery(tipo, fechaDesde, fechaHasta,
                venueId, estado, titulo, pagina, tamano));
        return Ok(result);
    }

    [HttpGet("{id:guid}/reporte")]
    [ProducesResponseType(typeof(ReporteOcupacionDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Reporte(Guid id)
    {
        var result = await _mediator.Send(new ReporteOcupacionQuery(id));
        return Ok(result);
    }
}