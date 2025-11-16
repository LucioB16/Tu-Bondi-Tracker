namespace TuBondi.Tests;

public sealed class TuBondiGatewayTests : IDisposable
{
    private readonly TransitMetrics _metrics = new();

    [Fact]
    public async Task Maps_arrivals_from_fixture()
    {
        var client = Substitute.For<ITuBondiClient>();
        client.CurrentPhpSessionId.Returns("cookie");
        var response = JsonSerializer.Deserialize<ArrivalsResponse>(TestData.ReadExample("06-arrivals.json"))!;
        client.GetArrivalsAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<IDictionary<int, bool>?>(), Arg.Any<CancellationToken>())
            .Returns(response);
        var gateway = CreateGateway(client);

        var snapshot = await gateway.GetArrivalsAsync("A911");

        snapshot.Stop.Code.Should().Be("A911");
        snapshot.Arrivals.Should().HaveCount(3);
        snapshot.Arrivals.First().Line.Should().Be("42");
        snapshot.Notifications.Should().ContainSingle(n => n.Line == "42");
    }

    [Fact]
    public async Task Retries_when_session_is_missing()
    {
        var client = Substitute.For<ITuBondiClient>();
        var response = JsonSerializer.Deserialize<ArrivalsResponse>(TestData.ReadExample("06-arrivals.json"))!;
        var callCounter = 0;
        client.GetArrivalsAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<IDictionary<int, bool>?>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                if (callCounter++ == 0)
                {
                    throw new InvalidOperationException("PHPSESSID missing");
                }

                return response;
            });
        client.CurrentPhpSessionId.Returns(callInfo => callCounter > 0 ? "cookie" : string.Empty);
        var gateway = CreateGateway(client);

        var snapshot = await gateway.GetArrivalsAsync("A911");

        snapshot.Arrivals.Should().NotBeEmpty();
        await client.Received(2).InitializeSessionAsync(Arg.Any<CancellationToken>());
        await client.Received(2).GetArrivalsAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<IDictionary<int, bool>?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Respects_cache_window()
    {
        var client = Substitute.For<ITuBondiClient>();
        var response = JsonSerializer.Deserialize<ArrivalsResponse>(TestData.ReadExample("06-arrivals.json"))!;
        client.CurrentPhpSessionId.Returns("cookie");
        client.GetArrivalsAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<IDictionary<int, bool>?>(), Arg.Any<CancellationToken>())
            .Returns(response);
        var options = Options.Create(new TuBondiGatewayOptions { CacheDurationSeconds = 0.05 });
        var gateway = CreateGateway(client, options);

        await gateway.GetArrivalsAsync("A911");
        await gateway.GetArrivalsAsync("A911");
        await Task.Delay(100);
        await gateway.GetArrivalsAsync("A911");

        await client.Received(2).GetArrivalsAsync(Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<bool>(), Arg.Any<IDictionary<int, bool>?>(), Arg.Any<CancellationToken>());
    }

    private TuBondiGateway CreateGateway(ITuBondiClient client, IOptions<TuBondiGatewayOptions>? options = null)
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var gatewayOptions = options ?? Options.Create(new TuBondiGatewayOptions());
        return new TuBondiGateway(client, cache, gatewayOptions, NullLogger<TuBondiGateway>.Instance, _metrics);
    }

    public void Dispose() => _metrics.Dispose();
}
