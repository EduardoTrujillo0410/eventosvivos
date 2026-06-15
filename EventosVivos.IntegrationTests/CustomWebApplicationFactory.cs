using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace EventosVivos.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remover TODOS los descriptores relacionados con DbContext y proveedores de BD
            var descriptorsToRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.FullName?.Contains("EntityFrameworkCore") == true &&
                     d.ServiceType.FullName?.Contains("Sqlite") == true))
                .ToList();

            foreach (var descriptor in descriptorsToRemove)
                services.Remove(descriptor);

            // Agregar InMemory limpio
            services.AddDbContext<AppDbContext>(opt =>
                opt.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}"));

            // Seed venues
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
            SeedVenues(db);
        });
    }

    private static void SeedVenues(AppDbContext db)
    {
        if (!db.Venues.Any())
        {
            db.Venues.AddRange(
                Venue.Create(1, "Auditorio Central", 200, "Bogotá"),
                Venue.Create(2, "Sala Norte", 50, "Bogotá"),
                Venue.Create(3, "Arena Sur", 500, "Medellín")
            );
            db.SaveChanges();
        }
    }
}