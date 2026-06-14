using EventosVivos.Application.DTOs;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Venues.Queries;

public record ListarVenuesQuery : IRequest<IEnumerable<VenueDto>>;

public class ListarVenuesHandler : IRequestHandler<ListarVenuesQuery, IEnumerable<VenueDto>>
{
    private readonly IVenueRepository _venues;
    public ListarVenuesHandler(IVenueRepository venues) => _venues = venues;

    public async Task<IEnumerable<VenueDto>> Handle(ListarVenuesQuery request, CancellationToken ct)
    {
        var venues = await _venues.GetAllAsync(ct);
        return venues.Select(v => new VenueDto(v.Id, v.Nombre, v.Capacidad, v.Ciudad));
    }
}