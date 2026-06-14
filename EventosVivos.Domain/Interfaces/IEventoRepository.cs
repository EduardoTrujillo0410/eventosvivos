using EventosVivos.Domain.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;

namespace EventosVivos.Domain.Interfaces;

public interface IEventoRepository
{
    Task<Evento?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Evento>> GetAllAsync(
        TipoEvento? tipo, DateTime? fechaDesde, DateTime? fechaHasta,
        int? venueId, EstadoEvento? estado, string? titulo,
        int pagina, int tamano,
        CancellationToken ct = default);
    Task AddAsync(Evento evento, CancellationToken ct = default);
    Task<bool> ExisteSuperposcicion(int venueId, DateTime inicio, DateTime fin,
        Guid? excludeId = null, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}