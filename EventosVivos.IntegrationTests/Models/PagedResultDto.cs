using System;
using System.Collections.Generic;
using System.Text;

namespace EventosVivos.IntegrationTests.Models
{
    public record PagedResultDto<T>(
    IEnumerable<T> Items,
    int TotalItems,
    int Pagina,
    int TamanoPage,
    int TotalPaginas,
    bool TieneSiguiente,
    bool TieneAnterior
    );
}
