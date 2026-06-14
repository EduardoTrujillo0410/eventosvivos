using EventosVivos.Domain.Entities;
using EventosVivos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventosVivos.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<Venue> Venues => Set<Venue>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Venue>(v =>
        {
            v.HasKey(x => x.Id);
            v.Property(x => x.Nombre).IsRequired().HasMaxLength(200);
            v.Property(x => x.Ciudad).IsRequired().HasMaxLength(100);
            v.HasData(
                new { Id = 1, Nombre = "Auditorio Central", Capacidad = 200, Ciudad = "Bogotá" },
                new { Id = 2, Nombre = "Sala Norte", Capacidad = 50, Ciudad = "Bogotá" },
                new { Id = 3, Nombre = "Arena Sur", Capacidad = 500, Ciudad = "Medellín" }
            );
        });

        modelBuilder.Entity<Evento>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Titulo).IsRequired().HasMaxLength(100);
            e.Property(x => x.Descripcion).IsRequired().HasMaxLength(500);
            e.Property(x => x.PrecioEntrada).HasColumnType("decimal(18,2)");
            e.Property(x => x.Tipo).HasConversion<string>();
            e.Property(x => x.Estado).HasConversion<string>();
            e.HasOne(x => x.Venue).WithMany().HasForeignKey(x => x.VenueId);
            // ← la relación con Reservas se define desde el lado de Reserva
        });

        modelBuilder.Entity<Reserva>(r =>
        {
            r.HasKey(x => x.Id);
            r.Property(x => x.NombreComprador).IsRequired().HasMaxLength(200);
            r.Property(x => x.EmailComprador).IsRequired().HasMaxLength(200);
            r.Property(x => x.CodigoReserva).HasMaxLength(20);
            r.Property(x => x.Estado).HasConversion<string>();
            // ← WithMany apunta a la colección Reservas de Evento
            r.HasOne(x => x.Evento)
             .WithMany(e => e.Reservas)
             .HasForeignKey(x => x.EventoId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}