using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using EventosVivos.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace EventosVivos.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var toRemove = services
                .Where(d =>
                    d.ServiceType.Namespace != null &&
                    (d.ServiceType.Namespace.Contains("EntityFrameworkCore") ||
                     d.ServiceType == typeof(AppDbContext) ||
                     d.ServiceType == typeof(DbContextOptions) ||
                     d.ServiceType == typeof(DbContextOptions<AppDbContext>)))
                .ToList();
            foreach (var d in toRemove)
                services.Remove(d);

            var authToRemove = services
                .Where(d => d.ServiceType.Namespace != null &&
                            d.ServiceType.Namespace.Contains("Authentication"))
                .ToList();
            foreach (var d in authToRemove)
                services.Remove(d);

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                opt.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

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