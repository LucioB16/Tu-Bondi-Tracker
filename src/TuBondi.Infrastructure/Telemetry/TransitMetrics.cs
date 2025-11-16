using System.Diagnostics.Metrics;

namespace TuBondi.Infrastructure.Telemetry;

/// <summary>
/// Instrumentación simple para medir la interacción con TuBondi.
/// </summary>
public sealed class TransitMetrics : IDisposable
{
    private readonly Meter _meter = new("TuBondi.Infrastructure", "1.0.0");
    private readonly Counter<long> _successCounter;
    private readonly Counter<long> _failureCounter;
    private readonly Histogram<double> _durationHistogram;

    public TransitMetrics()
    {
        _successCounter = _meter.CreateCounter<long>("tubondi_requests_success");
        _failureCounter = _meter.CreateCounter<long>("tubondi_requests_failure");
        _durationHistogram = _meter.CreateHistogram<double>("tubondi_requests_duration_ms");
    }

    public void RecordSuccess(TimeSpan duration)
    {
        _successCounter.Add(1);
        _durationHistogram.Record(duration.TotalMilliseconds);
    }

    public void RecordFailure(TimeSpan duration)
    {
        _failureCounter.Add(1);
        _durationHistogram.Record(duration.TotalMilliseconds);
    }

    public void Dispose() => _meter.Dispose();
}
