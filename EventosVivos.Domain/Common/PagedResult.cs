namespace EventosVivos.Domain.Common;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; init; } = [];
    public int TotalItems { get; init; }
    public int Pagina { get; init; }
    public int TamanoPage { get; init; }
    public int TotalPaginas => (int)Math.Ceiling((double)TotalItems / TamanoPage);
    public bool TieneSiguiente => Pagina < TotalPaginas;
    public bool TieneAnterior => Pagina > 1;
}