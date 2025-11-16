using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TuBondi.ApiClient.Http;
using TuBondi.ApiClient.Options;

namespace TuBondi.ApiClient.DependencyInjection;

/// <summary>
/// Métodos de extensión para registrar <see cref="ITuBondiClient"/> en contenedores DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registra <see cref="ITuBondiClient"/> como singleton reutilizando la configuración provista.
    /// </summary>
    /// <param name="services">Colección de servicios.</param>
    /// <param name="configure">Acción opcional para ajustar <see cref="TuBondiApiOptions"/>.</param>
    /// <returns>Instancia de <see cref="IServiceCollection"/> para encadenamiento.</returns>
    /// <remarks>
    /// Este método crea internamente un <see cref="TuBondiClient"/> con su propio <c>HttpClient</c> y <c>CookieContainer</c>.
    /// </remarks>
    public static IServiceCollection AddTuBondiApiClient(this IServiceCollection services, Action<TuBondiApiOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        services.TryAddSingleton(provider =>
        {
            var options = new TuBondiApiOptions();
            configure?.Invoke(options);
            return options;
        });
        services.TryAddSingleton<ITuBondiClient>(provider =>
        {
            var options = provider.GetRequiredService<TuBondiApiOptions>();
            return new TuBondiClient(options);
        });
        return services;
    }
}
