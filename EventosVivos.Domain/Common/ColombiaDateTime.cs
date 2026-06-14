namespace EventosVivos.Domain.Common;

public static class ColombiaDateTime
{
    private static readonly TimeZoneInfo _colombia =
        TimeZoneInfo.FindSystemTimeZoneById("America/Bogota");

    public static DateTime Now =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _colombia);
}