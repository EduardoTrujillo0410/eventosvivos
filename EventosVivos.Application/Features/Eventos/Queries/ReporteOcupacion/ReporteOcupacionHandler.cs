using EventosVivos.Application.DTOs;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Queries.ReporteOcupacion;

public class ReporteOcupacionHandler : IRequestHandler<ReporteOcupacionQuery, ReporteOcupacionDto>
{
    private readonly IEventoRepository _eventos;

    public ReporteOcupacionHandler(IEventoRepository eventos) => _eventos = eventos;

    public async Task<ReporteOcupacionDto> Handle(ReporteOcupacionQuery request, CancellationToken ct)
    {
        var evento = await _eventos.GetByIdAsync(request.EventoId, ct)
            ?? throw new DomainException($"Evento {request.EventoId} no encontrado.");

        evento.ActualizarEstadoSegunFecha();
        await _eventos.SaveChangesAsync(ct);

        var vendidas = evento.EntradasVendidas();
        var perdidas = evento.EntradasPerdidas();
        var disponibles = evento.EntradasDisponibles();
        var porcentaje = evento.CapacidadMaxima > 0
            ? (double)(vendidas + perdidas) / evento.CapacidadMaxima * 100
            : 0;
        var ingresos = vendidas * evento.PrecioEntrada;

        return new ReporteOcupacionDto(
            evento.Id, evento.Titulo, vendidas, disponibles,
            perdidas,
            Math.Round(porcentaje, 2), ingresos,
            evento.Estado.ToString().ToLower());
    }
}