using EventosVivos.Domain.Common;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;

namespace EventosVivos.Domain.Entities;

public class Reserva
{
    public Guid Id { get; private set; }
    public Guid EventoId { get; private set; }
    public Evento Evento { get; private set; } = null!;
    public int Cantidad { get; private set; }
    public string NombreComprador { get; private set; } = string.Empty;
    public string EmailComprador { get; private set; } = string.Empty;
    public EstadoReserva Estado { get; private set; }
    public string? CodigoReserva { get; private set; }
    public DateTime CreadaEn { get; private set; }
    public DateTime? CanceladaEn { get; private set; }
    public bool EsPerdida { get; private set; }

    private Reserva() { }

    public static Reserva Create(Evento evento, int cantidad, string nombre, string email)
    {
        if ((evento.FechaInicio - ColombiaDateTime.Now).TotalHours < 1)
            throw new DomainException("No se permiten reservas para eventos que inician en menos de 1 hora.");

        if ((evento.FechaInicio - ColombiaDateTime.Now).TotalHours < 24 && cantidad > 5)
            throw new DomainException("Con menos de 24 horas para el evento, solo se permiten máximo 5 entradas.");

        if (evento.PrecioEntrada > 100 && cantidad > 10)
            throw new DomainException("Para eventos con precio mayor a $100, el máximo es 10 entradas por transacción.");

        if (evento.EntradasDisponibles() < cantidad)
            throw new DomainException($"No hay suficientes entradas disponibles. Disponibles: {evento.EntradasDisponibles()}.");

        return new Reserva
        {
            Id = Guid.NewGuid(),
            EventoId = evento.Id,
            Evento = evento,
            Cantidad = cantidad,
            NombreComprador = nombre,
            EmailComprador = email,
            Estado = EstadoReserva.PendientePago,
            CreadaEn = ColombiaDateTime.Now
        };
    }

    public void ConfirmarPago(string codigoUnico)
    {
        if (Estado == EstadoReserva.Confirmada)
            throw new DomainException("La reserva ya está confirmada.");
        if (Estado == EstadoReserva.Cancelada)
            throw new DomainException("No se puede confirmar una reserva cancelada.");

        Estado = EstadoReserva.Confirmada;
        CodigoReserva = codigoUnico;
    }

    public void Cancelar(DateTime fechaInicio)
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new DomainException("La reserva ya está cancelada.");
        if (Estado == EstadoReserva.Confirmada)
            throw new DomainException("No se puede cancelar una reserva confirmada desde este método. Use CancelarConfirmada.");

        Estado = EstadoReserva.Cancelada;
        CanceladaEn = ColombiaDateTime.Now;
    }

    public void CancelarConfirmada(DateTime fechaInicio)
    {
        if (Estado == EstadoReserva.Cancelada)
            throw new DomainException("La reserva ya está cancelada.");

        if (Estado != EstadoReserva.Confirmada)
            throw new DomainException("Solo se pueden cancelar reservas confirmadas con este método.");

        if ((fechaInicio - ColombiaDateTime.Now).TotalHours < 48)
            EsPerdida = true;

        Estado = EstadoReserva.Cancelada;
        CanceladaEn = ColombiaDateTime.Now;
    }

    private static string GenerarCodigo()
    {
        var numero = Random.Shared.Next(0, 999999).ToString("D6");
        return $"EV-{numero}";
    }
}