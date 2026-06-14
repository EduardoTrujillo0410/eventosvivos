using EventosVivos.Application.DTOs;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using EventosVivos.Domain.Interfaces;
using MediatR;

namespace EventosVivos.Application.Features.Eventos.Commands.CrearEvento;

public class CrearEventoHandler : IRequestHandler<CrearEventoCommand, EventoDto>
{
    private readonly IEventoRepository _eventos;
    private readonly IVenueRepository _venues;

    public CrearEventoHandler(IEventoRepository eventos, IVenueRepository venues)
    {
        _eventos = eventos;
        _venues = venues;
    }

    public async Task<EventoDto> Handle(CrearEventoCommand request, CancellationToken ct)
    {
        var venue = await _venues.GetByIdAsync(request.VenueId, ct)
            ?? throw new DomainException($"Venue con ID {request.VenueId} no encontrado.");

        // RN-02: Superposición de venues
        var haySuperposcicion = await _eventos.ExisteSuperposcicion(
            request.VenueId, request.FechaInicio, request.FechaFin, null, ct);

        if (haySuperposcicion)
            throw new DomainException("Ya existe un evento activo en ese venue con horario superpuesto.");

        var tipo = Enum.Parse<TipoEvento>(request.Tipo, ignoreCase: true);

        var evento = Evento.Create(
            request.Titulo, request.Descripcion, request.VenueId, venue,
            request.CapacidadMaxima, request.FechaInicio, request.FechaFin,
            request.PrecioEntrada, tipo);

        await _eventos.AddAsync(evento, ct);
        await _eventos.SaveChangesAsync(ct);

        return MapToDto(evento);
    }

    public static EventoDto MapToDto(Evento e) => new(
        e.Id, e.Titulo, e.Descripcion, e.VenueId, e.Venue.Nombre,
        e.CapacidadMaxima, e.EntradasDisponibles(),
        e.FechaInicio, e.FechaFin, e.PrecioEntrada,
        e.Tipo.ToString().ToLower(), e.Estado.ToString().ToLower()
    );
}