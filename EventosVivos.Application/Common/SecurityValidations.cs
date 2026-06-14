namespace EventosVivos.Application.Common;

public static class SecurityValidations
{
    private static readonly string[] _patronesPeligrosos =
    [
        "--", ";--", ";", "/*", "*/", "xp_",
        "DROP ", "INSERT ", "DELETE ", "UPDATE ",
        "SELECT ", "EXEC ", "EXECUTE ",
        "<script", "javascript:", "onload="
    ];

    public static bool ContienePatronesSQL(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return false;
        var upper = input.ToUpperInvariant();
        return _patronesPeligrosos.Any(p => upper.Contains(p.ToUpperInvariant()));
    }
}