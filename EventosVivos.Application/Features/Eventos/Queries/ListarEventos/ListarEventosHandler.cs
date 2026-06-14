using EventosVivos.Application.DTOs;
using EventosVivos.Application.Features.Eventos.Commands.CrearEvento;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Queries.ListarEventos;

public class ListarEventosHandler : IRequestHandler<ListarEventosQuery, PagedResultDto<EventoDto>>
{
    private readonly IEventoRepository _eventos;

    public ListarEventosHandler(IEventoRepository eventos) => _eventos = eventos;

    public async Task<PagedResultDto<EventoDto>> Handle(ListarEventosQuery request, CancellationToken ct)
    {
        TipoEvento? tipo = request.Tipo != null
            ? Enum.Parse<TipoEvento>(request.Tipo, ignoreCase: true)
            : null;

        EstadoEvento? estado = request.Estado != null
            ? Enum.Parse<EstadoEvento>(request.Estado, ignoreCase: true)
            : null;

        var resultado = await _eventos.GetAllAsync(
            tipo, request.FechaDesde, request.FechaHasta,
            request.VenueId, estado, request.Titulo,
            request.Pagina, request.Tamano, ct);

        foreach (var e in resultado.Items)
            e.ActualizarEstadoSegunFecha();

        await _eventos.SaveChangesAsync(ct);

        return new PagedResultDto<EventoDto>(
            resultado.Items.Select(CrearEventoHandler.MapToDto),
            resultado.TotalItems,
            resultado.Pagina,
            resultado.TamanoPage,
            resultado.TotalPaginas,
            resultado.TieneSiguiente,
            resultado.TieneAnterior
        );
    }
}