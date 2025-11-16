using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Wrap;
using TuBondi.ApiClient.Http;
using TuBondi.ApiClient.Models;
using TuBondi.Application.Abstractions;
using TuBondi.Domain.Entities;
using TuBondi.Infrastructure.Options;
using TuBondi.Infrastructure.Telemetry;

namespace TuBondi.Infrastructure;

/// <summary>
/// Adaptador concreto que traduce la API TuBondi hacia el puerto definido por la aplicación.
/// </summary>
public sealed class TuBondiGateway : ITransitDataSource
{
    private static readonly Regex EtaExtractor = new("(?<digits>\\d+)", RegexOptions.Compiled | RegexOptions.CultureInvariant);
    private readonly ITuBondiClient _client;
    private readonly IMemoryCache _cache;
    private readonly ILogger<TuBondiGateway> _logger;
    private readonly TuBondiGatewayOptions _options;
    private readonly TransitMetrics _metrics;
    private readonly AsyncPolicyWrap<ArrivalsResponse> _policy;

    public TuBondiGateway(
        ITuBondiClient client,
        IMemoryCache cache,
        IOptions<TuBondiGatewayOptions> options,
        ILogger<TuBondiGateway> logger,
        TransitMetrics metrics)
    {
        _client = client;
        _cache = cache;
        _logger = logger;
        _metrics = metrics;
        _options = options.Value;
        _policy = BuildPolicy();
    }

    public Task InitializeAsync(CancellationToken ct = default)
        => _client.InitializeSessionAsync(ct);

    public async Task<BoardSnapshot> GetArrivalsAsync(string stopCode, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(stopCode))
        {
            throw new ArgumentException("Stop code is required", nameof(stopCode));
        }

        var cacheKey = $"arrivals::{stopCode.ToUpperInvariant()}";
        if (_cache.TryGetValue(cacheKey, out BoardSnapshot cachedSnapshot))
        {
            return cachedSnapshot;
        }

        var snapshot = await _cache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(Math.Max(1, _options.CacheDurationSeconds));
            return FetchSnapshotAsync(stopCode, ct);
        }) ?? throw new InvalidOperationException("Unable to materialize arrivals snapshot");

        return snapshot;
    }

    private async Task<BoardSnapshot> FetchSnapshotAsync(string stopCode, CancellationToken ct)
    {
        var attempt = 0;
        ArrivalsResponse response;
        while (true)
        {
            await EnsureSessionAsync(ct);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                response = await _policy.ExecuteAsync(token =>
                    _client.GetArrivalsAsync(stopCode, _options.Conf, _options.ShowExtendedHorizon, null, token), ct);
                stopwatch.Stop();
                _metrics.RecordSuccess(stopwatch.Elapsed);
                break;
            }
            catch (Exception ex) when (ShouldRetrySession(ex) && attempt < 1)
            {
                stopwatch.Stop();
                _metrics.RecordFailure(stopwatch.Elapsed);
                attempt++;
                _logger.LogWarning(ex, "TuBondi session lost. Re-initializing (attempt {Attempt})", attempt);
                await _client.InitializeSessionAsync(ct);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _metrics.RecordFailure(stopwatch.Elapsed);
                _logger.LogError(ex, "Unable to retrieve arrivals from TuBondi");
                throw;
            }
        }

        return MapResponse(response);
    }

    private async Task EnsureSessionAsync(CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(_client.CurrentPhpSessionId))
        {
            return;
        }

        _logger.LogDebug("Initializing TuBondi session");
        await _client.InitializeSessionAsync(ct);
    }

    private static bool ShouldRetrySession(Exception exception)
        => exception is InvalidOperationException || exception.Message.Contains("PHPSESSID", StringComparison.OrdinalIgnoreCase);

    private static AsyncPolicyWrap<ArrivalsResponse> BuildPolicy()
    {
        var jitter = new Random();
        var retry = Policy
            .Handle<Exception>(ex => ex is HttpRequestException or TaskCanceledException)
            .WaitAndRetryAsync(3, attempt =>
            {
                var baseDelay = TimeSpan.FromMilliseconds(200 * Math.Pow(2, attempt - 1));
                var extra = TimeSpan.FromMilliseconds(jitter.Next(50, 150));
                return baseDelay + extra;
            });

        var breaker = Policy
            .Handle<Exception>(ex => ex is HttpRequestException or TaskCanceledException)
            .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10));

        return Policy.WrapAsync(retry, breaker);
    }

    private static BoardSnapshot MapResponse(ArrivalsResponse response)
    {
        var stop = new StopSnapshot(
            response.Parada?.Codigo ?? string.Empty,
            response.Parada?.Descripcion ?? "Parada desconocida");

        var arrivals = response.ProximosArribos
            .Select(MapArrival)
            .ToList();

        var notifications = arrivals
            .Where(a => !string.IsNullOrWhiteSpace(a.Notification))
            .Select(a => new TransitNotification(a.Notification!, a.Line))
            .ToList();

        var error = ExtractError(response);

        return new BoardSnapshot(stop, arrivals, notifications, DateTimeOffset.UtcNow, error);
    }

    private static string? ExtractError(ArrivalsResponse response)
    {
        if (response.Err is JsonElement errElement && errElement.ValueKind is not JsonValueKind.Null and not JsonValueKind.Undefined)
        {
            return errElement.ToString();
        }

        return null;
    }

    private static TransitArrival MapArrival(Arribo arribo)
    {
        var eta = ParseEta(arribo.Demora ?? arribo.Proximo);
        double? distance = arribo.Distancia > 0 ? Math.Round(arribo.Distancia, 2) : null;
        var color = string.IsNullOrWhiteSpace(arribo.Color) ? "#ffb300" : arribo.Color;
        var direction = arribo.Sentido ?? arribo.RutaDescripcion ?? arribo.Ruta ?? string.Empty;
        var route = arribo.RutaDescripcion ?? arribo.Ruta ?? string.Empty;
        var op = arribo.ClienteNombre ?? "Operador";
        return new TransitArrival(
            arribo.Linea?.Trim() ?? "?",
            route,
            direction,
            op,
            distance,
            eta,
            color,
            arribo.Demora ?? arribo.Proximo,
            arribo.Notificacion);
    }

    private static int? ParseEta(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = value.Trim().ToLowerInvariant();
        if (normalized.Contains("arrib") || normalized.Contains("llega"))
        {
            return 0;
        }

        var match = EtaExtractor.Match(value);
        if (match.Success && int.TryParse(match.Groups["digits"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var minutes))
        {
            return minutes;
        }

        return null;
    }
}
