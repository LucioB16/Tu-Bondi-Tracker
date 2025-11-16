using Microsoft.Extensions.DependencyInjection;
using TuBondi.Application.Queries.GetArrivalsForBoard;

namespace TuBondi.Application.Extensions;

/// <summary>
/// Métodos de extensión para registrar los servicios de la capa de aplicación.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.AddScoped<IGetArrivalsForBoardQueryHandler, GetArrivalsForBoardQueryHandler>();
        return services;
    }
}
