namespace TuBondi.Tests;

public sealed class GetArrivalsForBoardQueryHandlerTests
{
    [Fact]
    public async Task Filters_and_sorts_arrivals_correctly()
    {
        var stop = new StopSnapshot("A911", "Test");
        var arrivals = new List<TransitArrival>
        {
            new("42", "R1", "Centro", "Operador", 1.2, 2, "#ffb300", "2min", null),
            new("40", "R2", "Barrio", "Operador", 2.2, 10, "#ffb300", "10min", null),
            new("80", "R3", "Otro", "Operador", 3, 1, "#ffb300", "1min", null)
        };
        var snapshot = new BoardSnapshot(stop, arrivals, Array.Empty<TransitNotification>(), DateTimeOffset.UtcNow, null);
        var dataSource = Substitute.For<ITransitDataSource>();
        dataSource.GetArrivalsAsync("A911", Arg.Any<CancellationToken>()).Returns(snapshot);
        var handler = new GetArrivalsForBoardQueryHandler(dataSource, NullLogger<GetArrivalsForBoardQueryHandler>.Instance);

        var result = await handler.HandleAsync(new GetArrivalsForBoardQuery("A911", new[] { "40", "42", "71" }, 2));

        result.Arrivals.Should().HaveCount(2);
        result.Arrivals.First().Line.Should().Be("42");
        result.Arrivals.Last().Line.Should().Be("40");
    }
}
