using System.Net.Http.Json;

namespace EventosVivos.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static async Task<string> ObtenerTokenAdmin(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@eventosvivos.com",
            password = "Admin123!"
        });

        var data = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return data!.Token;
    }

    public static async Task<string> ObtenerTokenUsuario(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "user@eventosvivos.com",
            password = "User123!"
        });

        var data = await response.Content.ReadFromJsonAsync<TokenResponse>();
        return data!.Token;
    }

    public static void AgregarToken(HttpClient client, string token)
        => client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
}

public record TokenResponse(string Token, string Rol, string Email);