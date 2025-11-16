using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TuBondi.ApiClient.DependencyInjection;
using TuBondi.Application.Abstractions;
using TuBondi.Infrastructure.Options;
using TuBondi.Infrastructure.Telemetry;

namespace TuBondi.Infrastructure.DependencyInjection;

/// <summary>
/// Registra las dependencias concretas de infraestructura.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        services.AddMemoryCache();
        services.AddSingleton<TransitMetrics>();

        services.Configure<TuBondiGatewayOptions>(configuration.GetSection("TuBondi"));

        services.AddTuBondiApiClient(options =>
        {
            var baseUrl = configuration["TuBondi:BaseUrl"];
            if (!string.IsNullOrWhiteSpace(baseUrl))
            {
                options.BaseAddress = new Uri(baseUrl, UriKind.Absolute);
            }

            var conf = configuration["TuBondi:Conf"];
            if (!string.IsNullOrWhiteSpace(conf))
            {
                options.Conf = conf;
            }

            var ua = configuration["TuBondi:UserAgent"] ?? "CordobaTransitTracker/1.0 (+https://transit-tracker.eastsideurbanism.org/)";
            options.UserAgent = ua;
        });

        services.AddSingleton<ITransitDataSource, TuBondiGateway>();

        return services;
    }
}
