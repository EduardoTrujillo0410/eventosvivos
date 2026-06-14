using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Repositories;

public class VenueRepository : IVenueRepository
{
    private readonly AppDbContext _db;
    public VenueRepository(AppDbContext db) => _db = db;

    public async Task<Venue?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Venues.FindAsync(new object[] { id }, ct);

    public async Task<IEnumerable<Venue>> GetAllAsync(CancellationToken ct = default)
        => await _db.Venues.ToListAsync(ct);
}