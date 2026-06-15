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

    private static int _diaCounter = 10;
    private async Task<EventoDto> CrearEventoBase(decimal precio = 50m,
        int capacidad = 100, int diasDesdeHoy = 0)
    {
        var token = await AuthHelper.ObtenerTokenAdmin(_client);
        AuthHelper.AgregarToken(_client, token);

        // Si no se especifica día, usar uno único para evitar superposición
        var dias = diasDesdeHoy == 0 ? Interlocked.Increment(ref _diaCounter) + 30 : diasDesdeHoy;

        var response = await _client.PostAsJsonAsync("/api/eventos", new
        {
            titulo = "Evento Base para Reservas",
            descripcion = "Descripción válida con suficientes caracteres aquí",
            venueId = 1,
            capacidadMaxima = capacidad,
            fechaInicio = DateTime.UtcNow.AddDays(dias).ToString("o"),
            fechaFin = DateTime.UtcNow.AddDays(dias).AddHours(3).ToString("o"),
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

}