using EventosVivos.Application.DTOs;
using EventosVivos.IntegrationTests;
using EventosVivos.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace EventosVivos.IntegrationTests;

public class EventosIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventosIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task AutenticarComoAdmin()
    {
        var token = await AuthHelper.ObtenerTokenAdmin(_client);
        AuthHelper.AgregarToken(_client, token);
    }

    private static object EventoValido(int venueId = 1, string tipo = "conferencia",
        decimal precio = 50m, int capacidad = 100, int diasDesdeHoy = 5) => new
        {
            titulo = "Conferencia de Tecnología",
            descripcion = "Descripción válida del evento con más de diez caracteres",
            venueId,
            capacidadMaxima = capacidad,
            fechaInicio = DateTime.UtcNow.AddDays(diasDesdeHoy).ToString("o"),
            fechaFin = DateTime.UtcNow.AddDays(diasDesdeHoy).AddHours(3).ToString("o"),
            precioEntrada = precio,
            tipo
        };


    [Fact]
    public async Task RN01_CrearEvento_CapacidadSuperiorVenue_Debe_Retornar422()
    {
        await AutenticarComoAdmin();

        var response = await _client.PostAsJsonAsync("/api/eventos",
            EventoValido(venueId: 2, capacidad: 999));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RN02_CrearEvento_SuperposicionVenue_Debe_Retornar422()
    {
        await AutenticarComoAdmin();

        await _client.PostAsJsonAsync("/api/eventos", EventoValido(venueId: 1));

        var response = await _client.PostAsJsonAsync("/api/eventos", EventoValido(venueId: 1));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RN03_CrearEvento_FinSemana_Despues22h_Debe_Retornar422()
    {
        await AutenticarComoAdmin();

        var proximoSabado = ProximoDia(DayOfWeek.Saturday).Date.AddHours(23);

        var response = await _client.PostAsJsonAsync("/api/eventos", new
        {
            titulo = "Concierto Nocturno Test",
            descripcion = "Descripción suficientemente larga para pasar validación",
            venueId = 1,
            capacidadMaxima = 100,
            fechaInicio = proximoSabado.ToString("o"),
            fechaFin = proximoSabado.AddHours(2).ToString("o"),
            precioEntrada = 80,
            tipo = "concierto"
        });

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task RF02_ListarEventos_SinFiltros_Debe_Retornar200()
    {
        await AutenticarComoAdmin();
        await _client.PostAsJsonAsync("/api/eventos", EventoValido(diasDesdeHoy: 10));

        var response = await _client.GetAsync("/api/eventos");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var resultado = await response.Content.ReadFromJsonAsync<PagedResultDto<EventoDto>>();
        resultado.Should().NotBeNull();
    }

    [Fact]
    public async Task RF02_ListarEventos_FiltroPorTipo_DebeRetornarSoloEseTipo()
    {
        await AutenticarComoAdmin();
        await _client.PostAsJsonAsync("/api/eventos", EventoValido(tipo: "taller", diasDesdeHoy: 15));

        var response = await _client.GetAsync("/api/eventos?tipo=taller");
        var resultado = await response.Content.ReadFromJsonAsync<PagedResultDto<EventoDto>>();
        resultado!.Items.Should().AllSatisfy(e => e.Tipo.Should().Be("taller"));
    }

    private static DateTime ProximoDia(DayOfWeek dia)
    {
        var hoy = DateTime.UtcNow;
        int diff = ((int)dia - (int)hoy.DayOfWeek + 7) % 7;
        if (diff == 0) diff = 7;
        return hoy.AddDays(diff);
    }
}