namespace EventosVivos.Application.DTOs;

public record ReservaDto(
    Guid Id,
    Guid EventoId,
    string EventoTitulo,
    int Cantidad,
    string NombreComprador,
    string EmailComprador,
    string Estado,
    string? CodigoReserva,
    DateTime CreadaEn,
    DateTime? CanceladaEn
);

public record CrearReservaRequest(
    Guid EventoId,
    int Cantidad,
    string NombreComprador,
    string EmailComprador
);

public record ReporteOcupacionDto(
    Guid EventoId,
    string EventoTitulo,
    int EntradasVendidas,
    int EntradasDisponibles,
    int EntradasPerdidas,
    double PorcentajeOcupacion,
    decimal TotalIngresos,
    string EstadoEvento
);