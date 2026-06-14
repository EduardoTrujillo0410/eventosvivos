using EventosVivos.Domain.Entities;

namespace EventosVivos.Domain.Interfaces;

public interface IReservaRepository
{
    Task<Reserva?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Reserva reserva, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<IEnumerable<Reserva>> GetByEventoIdAsync(Guid eventoId, CancellationToken ct = default);
    Task<bool> ExisteCodigoReserva(string codigo, CancellationToken ct = default);
}