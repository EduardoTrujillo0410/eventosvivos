using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces;

public interface IVenueRepository
{
    Task<Venue?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Venue>> GetAllAsync(CancellationToken ct = default);
}