using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Venues.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventosVivos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VenuesController : ControllerBase
{
    private readonly IMediator _mediator;
    public VenuesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VenueDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar()
    {
        var result = await _mediator.Send(new ListarVenuesQuery());
        return Ok(result);
    }
}