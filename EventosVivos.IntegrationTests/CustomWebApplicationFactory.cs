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
            // Reemplazar solo el DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                opt.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            // Deshabilitar Rate Limiter
            var rateLimitDescriptors = services
                .Where(d => d.ServiceType.FullName != null &&
                            d.ServiceType.FullName.Contains("RateLimiting"))
                .ToList();
            foreach (var d in rateLimitDescriptors) services.Remove(d);

            // Auth fake
            var authDescriptors = services
                .Where(d => d.ServiceType.Namespace != null &&
                            d.ServiceType.Namespace.Contains("Authentication"))
                .ToList();
            foreach (var d in authDescriptors) services.Remove(d);

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            // Seed
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