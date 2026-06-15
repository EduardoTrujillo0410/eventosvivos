using EventosVivos.Domain.Common;
using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

public class EventoRepository : IEventoRepository
{
    private readonly AppDbContext _db;
    public EventoRepository(AppDbContext db) => _db = db;

    public async Task<Evento?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Eventos
            .Include(e => e.Venue)
            .Include(e => e.Reservas)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<PagedResult<Evento>> GetAllAsync(
    TipoEvento? tipo, DateTime? fechaDesde, DateTime? fechaHasta,
    int? venueId, EstadoEvento? estado, string? titulo,
    int pagina, int tamano,
    CancellationToken ct = default)
    {
        pagina = Math.Max(1, pagina);
        tamano = Math.Clamp(tamano, 1, 50);

        var query = _db.Eventos.Include(e => e.Venue).Include(e => e.Reservas).AsQueryable();

        if (tipo.HasValue) query = query.Where(e => e.Tipo == tipo);
        if (fechaDesde.HasValue) query = query.Where(e => e.FechaInicio >= fechaDesde);
        if (fechaHasta.HasValue) query = query.Where(e => e.FechaInicio <= fechaHasta);
        if (venueId.HasValue) query = query.Where(e => e.VenueId == venueId);
        if (estado.HasValue) query = query.Where(e => e.Estado == estado);
        if (!string.IsNullOrWhiteSpace(titulo))
            query = query.Where(e => e.Titulo.ToLower().Contains(titulo.ToLower()));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderBy(e => e.FechaInicio)
            .Skip((pagina - 1) * tamano)
            .Take(tamano)
            .ToListAsync(ct);

        return new PagedResult<Evento>
        {
            Items = items,
            TotalItems = total,
            Pagina = pagina,
            TamanoPage = tamano
        };
    }

    public async Task AddAsync(Evento evento, CancellationToken ct = default)
        => await _db.Eventos.AddAsync(evento, ct);

    public async Task<bool> ExisteSuperposcicion(int venueId, DateTime inicio, DateTime fin,
        Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = _db.Eventos
            .Where(e => e.VenueId == venueId
                && e.Estado == EstadoEvento.Activo
                && inicio < e.FechaFin
                && fin > e.FechaInicio);

        if (excludeId.HasValue)
            query = query.Where(e => e.Id != excludeId);

        return await query.AnyAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);
}