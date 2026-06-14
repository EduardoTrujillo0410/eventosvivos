using EventosVivos.Domain.Enums;

namespace EventosVivos.Application.DTOs;

public record EventoDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    int VenueId,
    string VenueNombre,
    int CapacidadMaxima,
    int EntradasDisponibles,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal PrecioEntrada,
    string Tipo,
    string Estado
);

public record CrearEventoRequest(
    string Titulo,
    string Descripcion,
    int VenueId,
    int CapacidadMaxima,
    DateTime FechaInicio,
    DateTime FechaFin,
    decimal PrecioEntrada,
    string Tipo
);

public record FiltrosEventoRequest(
    string? Tipo,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int? VenueId,
    string? Estado,
    string? Titulo
);

public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int TotalItems,
    int Pagina,
    int TamanoPage,
    int TotalPaginas,
    bool TieneSiguiente,
    bool TieneAnterior
);