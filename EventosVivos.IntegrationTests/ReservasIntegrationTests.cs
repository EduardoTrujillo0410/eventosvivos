using EventosVivos.Application.DTOs;
using EventosVivos.IntegrationTests;
using EventosVivos.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace EventosVivos.IntegrationTests;

public class ReservasIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ReservasIntegrationTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient();

    private async Task<EventoDto> CrearEventoBase(decimal precio = 50m,
        int capacidad = 100, int diasDesdeHoy = 5)
    {
        var token = await AuthHelper.ObtenerTokenAdmin(_client);
        AuthHelper.AgregarToken(_client, token);

        var response = await _client.PostAsJsonAsync("/api/eventos", new
        {
            titulo = "Evento Base para Reservas",
            descripcion = "Descripción válida con suficientes caracteres aquí",
            venueId = 1,
            capacidadMaxima = capacidad,
            fechaInicio = DateTime.UtcNow.AddDays(diasDesdeHoy).ToString("o"),
            fechaFin = DateTime.UtcNow.AddDays(diasDesdeHoy).AddHours(3).ToString("o"),
            precioEntrada = precio,
            tipo = "conferencia"
        });

        return (await response.Content.ReadFromJsonAsync<EventoDto>())!;
    }

    private static object ReservaValida(Guid eventoId, int cantidad = 2) => new
    {
        eventoId,
        cantidad,
        nombreComprador = "Juan Pérez",
        emailComprador = "juan@test.com"
    };

    [Fact]
    public async Task RF03_CrearReserva_Valida_Debe_Retornar201()
    {
        var evento = await CrearEventoBase();

        var response = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var reserva = await response.Content.ReadFromJsonAsync<ReservaDto>();
        reserva!.Estado.Should().Be("PendientePago");
        reserva.CodigoReserva.Should().BeNull();
    }

    [Fact]
    public async Task RF03_CrearReserva_EmailInvalido_Debe_Retornar400()
    {
        var evento = await CrearEventoBase();

        var response = await _client.PostAsJsonAsync("/api/reservas", new
        {
            eventoId = evento.Id,
            cantidad = 1,
            nombreComprador = "Test",
            emailComprador = "no-es-un-email"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RF03_CrearReserva_SinDisponibilidad_Debe_Retornar422()
    {
        var evento = await CrearEventoBase(capacidad: 2);

        await _client.PostAsJsonAsync("/api/reservas", ReservaValida(evento.Id, cantidad: 2));

        var response = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id, cantidad: 1));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RN04_CrearReserva_MenosDeUnaHora_Debe_Retornar422()
    {
        var token = await AuthHelper.ObtenerTokenAdmin(_client);
        AuthHelper.AgregarToken(_client, token);

        var response = await _client.PostAsJsonAsync("/api/reservas", new
        {
            eventoId = Guid.NewGuid(),
            cantidad = 1,
            nombreComprador = "Test",
            emailComprador = "test@test.com"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RN05_CrearReserva_Precio100_Mas10Entradas_Debe_Retornar422()
    {
        var evento = await CrearEventoBase(precio: 150m, capacidad: 100);

        var response = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id, cantidad: 11));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RF04_ConfirmarPago_Debe_GenerarCodigoEV()
    {
        var evento = await CrearEventoBase();
        var reservaResp = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id));
        var reserva = (await reservaResp.Content.ReadFromJsonAsync<ReservaDto>())!;

        var response = await _client.PatchAsync(
            $"/api/reservas/{reserva.Id}/confirmar-pago", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var confirmada = await response.Content.ReadFromJsonAsync<ReservaDto>();
        confirmada!.Estado.Should().Be("Confirmada");
        confirmada.CodigoReserva.Should().MatchRegex(@"^EV-\d{6}$");
    }

    [Fact]
    public async Task RF04_ConfirmarPago_YaConfirmada_Debe_Retornar422()
    {
        var evento = await CrearEventoBase();
        var reservaResp = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id));
        var reserva = (await reservaResp.Content.ReadFromJsonAsync<ReservaDto>())!;

        await _client.PatchAsync($"/api/reservas/{reserva.Id}/confirmar-pago", null);

        var response = await _client.PatchAsync(
            $"/api/reservas/{reserva.Id}/confirmar-pago", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RF05_CancelarReserva_Pendiente_Debe_Retornar200()
    {
        var evento = await CrearEventoBase();
        var reservaResp = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id));
        var reserva = (await reservaResp.Content.ReadFromJsonAsync<ReservaDto>())!;

        var response = await _client.PatchAsync(
            $"/api/reservas/{reserva.Id}/cancelar", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cancelada = await response.Content.ReadFromJsonAsync<ReservaDto>();
        cancelada!.Estado.Should().Be("Cancelada");
        cancelada.CanceladaEn.Should().NotBeNull();
    }

    [Fact]
    public async Task RF05_CancelarReserva_YaCancelada_Debe_Retornar422()
    {
        var evento = await CrearEventoBase();
        var reservaResp = await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id));
        var reserva = (await reservaResp.Content.ReadFromJsonAsync<ReservaDto>())!;

        await _client.PatchAsync($"/api/reservas/{reserva.Id}/cancelar", null);

        var response = await _client.PatchAsync(
            $"/api/reservas/{reserva.Id}/cancelar", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RF06_ReporteOcupacion_Debe_CalcularCorrectamente()
    {
        var evento = await CrearEventoBase(precio: 100m, capacidad: 10);

        var r1 = (await (await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id, cantidad: 3))).Content.ReadFromJsonAsync<ReservaDto>())!;
        var r2 = (await (await _client.PostAsJsonAsync("/api/reservas",
            ReservaValida(evento.Id, cantidad: 2))).Content.ReadFromJsonAsync<ReservaDto>())!;

        await _client.PatchAsync($"/api/reservas/{r1.Id}/confirmar-pago", null);
        await _client.PatchAsync($"/api/reservas/{r2.Id}/confirmar-pago", null);

        var response = await _client.GetAsync($"/api/eventos/{evento.Id}/reporte");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var reporte = await response.Content.ReadFromJsonAsync<ReporteOcupacionDto>();
        reporte!.EntradasVendidas.Should().Be(5);
        reporte.TotalIngresos.Should().Be(500m);
        reporte.PorcentajeOcupacion.Should().Be(50);
    }
}