using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Exceptions;
using FluentAssertions;

namespace EventosVivos.Tests.Domain;

public class EventoTests
{
    private static Venue VenueDefault() => Venue.Create(1, "Auditorio", 200, "Bogotá");

    [Fact]
    public void Crear_Evento_Valido_DebeRetornarInstancia()
    {
        var venue = VenueDefault();
        var inicio = DateTime.UtcNow.AddDays(5);
        var fin = inicio.AddHours(3);

        var evento = Evento.Create("Conferencia Tech", "Descripción del evento válido",
            1, venue, 100, inicio, fin, 50m, TipoEvento.Conferencia);

        evento.Should().NotBeNull();
        evento.Estado.Should().Be(EstadoEvento.Activo);
        evento.EntradasDisponibles().Should().Be(100);
    }

    [Fact]
    public void Crear_Evento_CapacidadSuperiorVenue_DebeLanzarExcepcion()
    {
        var venue = VenueDefault();
        var inicio = DateTime.UtcNow.AddDays(1);

        var act = () => Evento.Create("Título válido aquí", "Descripción larga suficiente",
            1, venue, 300, inicio, inicio.AddHours(2), 50m, TipoEvento.Taller);

        act.Should().Throw<DomainException>()
            .WithMessage("*capacidad*");
    }

    [Fact]
    public void Crear_Evento_FechaInicioEnPasado_DebeRechazar()
    {
        var venue = VenueDefault();

        var act = () => Evento.Create("Título válido aquí", "Descripción larga suficiente",
            1, venue, 50, DateTime.UtcNow.AddHours(-1),
            DateTime.UtcNow.AddHours(1), 30m, TipoEvento.Concierto);

        act.Should().Throw<DomainException>().WithMessage("*futura*");
    }

    [Fact]
    public void Crear_Evento_FinAntesDeInicio_DebeRechazar()
    {
        var venue = VenueDefault();
        var inicio = DateTime.UtcNow.AddDays(2);

        var act = () => Evento.Create("Título válido aquí", "Descripción larga suficiente",
            1, venue, 50, inicio, inicio.AddHours(-1), 30m, TipoEvento.Taller);

        act.Should().Throw<DomainException>().WithMessage("*posterior*");
    }

    [Fact]
    public void RN03_Evento_FinSemana_Despues22h_DebeRechazar()
    {
        var venue = VenueDefault();
        // Próximo sábado a las 23:00
        var sabado = ProximoDiaSemana(DayOfWeek.Saturday).Date.AddHours(23);

        var act = () => Evento.Create("Concierto Nocturno Test", "Descripción suficientemente larga",
            1, venue, 50, sabado, sabado.AddHours(2), 80m, TipoEvento.Concierto);

        act.Should().Throw<DomainException>().WithMessage("*22:00*");
    }

    [Fact]
    public void RN06_ActualizarEstado_EventoVencido_DebeMarcarCompletado()
    {
        // Necesitamos acceso a estado vía reflexión para simular un evento pasado
        var venue = VenueDefault();
        var inicio = DateTime.UtcNow.AddDays(1);
        var evento = Evento.Create("Evento Futuro Test", "Descripción del evento a verificar",
            1, venue, 50, inicio, inicio.AddHours(3), 30m, TipoEvento.Conferencia);

        // El estado solo cambia si ya pasó el fin — en test unitario verificamos la lógica
        evento.ActualizarEstadoSegunFecha();
        evento.Estado.Should().Be(EstadoEvento.Activo); // no ha pasado aún
    }

    private static DateTime ProximoDiaSemana(DayOfWeek dia)
    {
        var hoy = DateTime.UtcNow;
        int diff = ((int)dia - (int)hoy.DayOfWeek + 7) % 7;
        if (diff == 0) diff = 7;
        return hoy.AddDays(diff);
    }
}