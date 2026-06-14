using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EventosVivos.Domain.Interfaces;
using EventosVivos.Infrastructure.Persistence;
using EventosVivos.Infrastructure.Repositories;

namespace EventosVivos.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();
        services.AddScoped<IVenueRepository, VenueRepository>();

        return services;
    }
}