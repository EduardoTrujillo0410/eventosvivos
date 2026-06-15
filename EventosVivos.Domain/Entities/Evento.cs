using EventosVivos.Domain.Common;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Entities;

public class Evento
{
    public Guid Id { get; private set; }
    public string Titulo { get; private set; } = string.Empty;
    public string Descripcion { get; private set; } = string.Empty;
    public int VenueId { get; private set; }
    public Venue Venue { get; private set; } = null!;
    public int CapacidadMaxima { get; private set; }
    public DateTime FechaInicio { get; private set; }
    public DateTime FechaFin { get; private set; }
    public decimal PrecioEntrada { get; private set; }
    public TipoEvento Tipo { get; private set; }
    public EstadoEvento Estado { get; private set; }
    public DateTime CreadoEn { get; private set; }

    private List<Reserva> _reservas = new();
    public IReadOnlyCollection<Reserva> Reservas => _reservas.AsReadOnly();

    private Evento() { }

    public static Evento Create(
        string titulo, string descripcion, int venueId, Venue venue,
        int capacidadMaxima, DateTime fechaInicio, DateTime fechaFin,
        decimal precioEntrada, TipoEvento tipo)
    {
        if (capacidadMaxima > venue.Capacidad)
            throw new DomainException($"La capacidad máxima no puede exceder la del venue ({venue.Capacidad}).");

        if (fechaInicio <= ColombiaDateTime.Now)
            throw new DomainException("La fecha de inicio debe ser futura.");

        if (fechaFin <= fechaInicio)
            throw new DomainException("La fecha de fin debe ser posterior al inicio.");

        if ((fechaInicio.DayOfWeek == DayOfWeek.Saturday || fechaInicio.DayOfWeek == DayOfWeek.Sunday)
            && fechaInicio.Hour >= 22)
            throw new DomainException("Los eventos en fin de semana no pueden iniciar después de las 22:00.");

        return new Evento
        {
            Id = Guid.NewGuid(),
            Titulo = titulo,
            Descripcion = descripcion,
            VenueId = venueId,
            Venue = venue,
            CapacidadMaxima = capacidadMaxima,
            FechaInicio = TimeZoneInfo.ConvertTimeFromUtc(
                fechaInicio.ToUniversalTime(),
                TimeZoneInfo.FindSystemTimeZoneById("America/Bogota")),
            FechaFin = TimeZoneInfo.ConvertTimeFromUtc(
                fechaFin.ToUniversalTime(),
                TimeZoneInfo.FindSystemTimeZoneById("America/Bogota")),
            PrecioEntrada = precioEntrada,
            Tipo = tipo,
            Estado = EstadoEvento.Activo,
            CreadoEn = ColombiaDateTime.Now
        };
    }

    public int EntradasDisponibles()
    {
        var reservadas = _reservas
            .Where(r => r.Estado == EstadoReserva.Confirmada ||
                        r.Estado == EstadoReserva.PendientePago ||
                        (r.Estado == EstadoReserva.Cancelada && r.EsPerdida))
            .Sum(r => r.Cantidad);
        return CapacidadMaxima - reservadas;
    }

    public int EntradasVendidas()
        => _reservas.Where(r => r.Estado == EstadoReserva.Confirmada).Sum(r => r.Cantidad);

    public int EntradasPerdidas()
    => _reservas.Where(r => r.Estado == EstadoReserva.Cancelada && r.EsPerdida).Sum(r => r.Cantidad);


    public void ActualizarEstadoSegunFecha()
    {
        if (Estado == EstadoEvento.Cancelado) return;
        if (ColombiaDateTime.Now > FechaFin)
            Estado = EstadoEvento.Completado;
    }

    public bool TieneSuperposcicion(DateTime inicio, DateTime fin)
        => Estado == EstadoEvento.Activo &&
           inicio < FechaFin && fin > FechaInicio;

    public void Cancelar() => Estado = EstadoEvento.Cancelado;

    public void AgregarReservaParaTest(Reserva reserva)
    => _reservas.Add(reserva);
}