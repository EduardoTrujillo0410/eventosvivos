using EventosVivos.Domain.Entities;
using EventosVivos.Infrastructure.Persistence;
using EventosVivos.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace EventosVivos.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var efDescriptors = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(AppDbContext) ||
                    (d.ServiceType.FullName != null &&
                     d.ServiceType.FullName.StartsWith("Microsoft.EntityFrameworkCore")))
                .ToList();

            foreach (var d in efDescriptors)
                services.Remove(d);

            services.AddDbContext<AppDbContext>((sp, opt) =>
            {
                opt.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                opt.ConfigureWarnings(w =>
                    w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            });

            var authDescriptors = services
                .Where(d => d.ServiceType.Namespace != null &&
                            d.ServiceType.Namespace.Contains("Authentication"))
                .ToList();
            foreach (var d in authDescriptors)
                services.Remove(d);

            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

            var sp2 = services.BuildServiceProvider();
            using var scope = sp2.CreateScope();
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