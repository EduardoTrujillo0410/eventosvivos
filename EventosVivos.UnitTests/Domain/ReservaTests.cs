using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using FluentAssertions;

namespace EventosVivos.Tests.Domain;

public class ReservaTests
{
    private static Evento EventoActivo(decimal precio = 50m, int capacidad = 100)
    {
        var venue = Venue.Create(1, "Sala Test", 200, "Bogotá");
        var inicio = DateTime.UtcNow.AddDays(5);
        return Evento.Create("Evento Test Valid", "Descripción de prueba suficiente",
            1, venue, capacidad, inicio, inicio.AddHours(3), precio, TipoEvento.Conferencia);
    }

    [Fact]
    public void Crear_Reserva_Valida_DebeCrearEnEstadoPendientePago()
    {
        var evento = EventoActivo();
        var reserva = Reserva.Create(evento, 2, "Juan Pérez", "juan@email.com");

        reserva.Estado.Should().Be(EstadoReserva.PendientePago);
        reserva.Cantidad.Should().Be(2);
        reserva.CodigoReserva.Should().BeNull();
    }

    [Fact]
    public void RF03_Reserva_SinDisponibilidad_DebeRechazar()
    {
        var evento = EventoActivo(capacidad: 1);
        Reserva.Create(evento, 1, "A", "a@b.com"); // ocupa la única entrada

        var act = () => Reserva.Create(evento, 1, "B", "b@c.com");
        act.Should().Throw<DomainException>().WithMessage("*disponibles*");
    }

    [Fact]
    public void RN05_Precio_Mayor100_Mas10Entradas_DebeRechazar()
    {
        var evento = EventoActivo(precio: 150m, capacidad: 100);

        var act = () => Reserva.Create(evento, 11, "Carlos", "carlos@test.com");
        act.Should().Throw<DomainException>().WithMessage("*100*");
    }

    [Fact]
    public void RF04_ConfirmarPago_DebeGenerarCodigo()
    {
        var evento = EventoActivo();
        var reserva = Reserva.Create(evento, 1, "Ana", "ana@test.com");

        reserva.ConfirmarPago();

        reserva.Estado.Should().Be(EstadoReserva.Confirmada);
        reserva.CodigoReserva.Should().MatchRegex(@"^EV-\d{6}$");
    }

    [Fact]
    public void RF04_ConfirmarPago_YaConfirmada_DebeRechazar()
    {
        var evento = EventoActivo();
        var reserva = Reserva.Create(evento, 1, "Ana", "ana@test.com");
        reserva.ConfirmarPago();

        var act = () => reserva.ConfirmarPago();
        act.Should().Throw<DomainException>().WithMessage("*confirmada*");
    }

    [Fact]
    public void RF05_CancelarReservaPendiente_DebeLiberar()
    {
        var evento = EventoActivo();
        var reserva = Reserva.Create(evento, 3, "Pedro", "pedro@test.com");

        reserva.Cancelar(evento.FechaInicio);

        reserva.Estado.Should().Be(EstadoReserva.Cancelada);
        reserva.CanceladaEn.Should().NotBeNull();
    }

    [Fact]
    public void RN07_CancelarConfirmada_Menos48h_DebeMarcarPerdida()
    {
        var evento = EventoActivo();
        var reserva = Reserva.Create(evento, 1, "Luis", "luis@test.com");
        reserva.ConfirmarPago();

        // Simular cancelación con menos de 48h
        var fechaInicio = DateTime.UtcNow.AddHours(10);
        reserva.CancelarConfirmada(fechaInicio);

        reserva.EsPerdida.Should().BeTrue();
        reserva.Estado.Should().Be(EstadoReserva.Cancelada);
    }
}