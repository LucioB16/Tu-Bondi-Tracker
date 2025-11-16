namespace TuBondi.Tests;

public sealed class BoardClientConfigProviderTests
{
    [Fact]
    public void Creates_expected_cadence()
    {
        var options = Options.Create(new TrackerOptions
        {
            StopCode = "A911",
            StopDescription = "Demo",
            Lines = "40,42,71",
            UpdateIntervalSeconds = 30
        });
        var provider = new BoardClientConfigProvider(options);

        var config = provider.GetConfig();

        config.UpdateInterval.Should().Be(TimeSpan.FromSeconds(30));
        config.Lines.Should().Contain(new[] { "40", "42", "71" });
    }
}
