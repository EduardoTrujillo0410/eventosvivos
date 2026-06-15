using System.Net.Http.Headers;

namespace EventosVivos.IntegrationTests.Helpers;

public static class AuthHelper
{
    public static void AgregarToken(HttpClient client, string token)
        => client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

    // Sin JWT real — los tests no necesitan auth si los endpoints no lo requieren
    public static Task<string> ObtenerTokenAdmin(HttpClient client)
        => Task.FromResult("test-token-admin");

    public static Task<string> ObtenerTokenUsuario(HttpClient client)
        => Task.FromResult("test-token-usuario");
}