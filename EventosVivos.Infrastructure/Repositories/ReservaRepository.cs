using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly AppDbContext _db;
    public ReservaRepository(AppDbContext db) => _db = db;

    public async Task<Reserva?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Reservas
            .Include(r => r.Evento).ThenInclude(e => e.Venue)
            .FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task AddAsync(Reserva reserva, CancellationToken ct = default)
        => await _db.Reservas.AddAsync(reserva, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public async Task<IEnumerable<Reserva>> GetByEventoIdAsync(Guid eventoId, CancellationToken ct = default)
    => await _db.Reservas
        .Include(r => r.Evento).ThenInclude(e => e.Venue)
        .Where(r => r.EventoId == eventoId)
        .OrderByDescending(r => r.CreadaEn)
        .ToListAsync(ct);

    public async Task<bool> ExisteCodigoReserva(string codigo, CancellationToken ct = default)
    => await _db.Reservas.AnyAsync(r => r.CodigoReserva == codigo, ct);
}