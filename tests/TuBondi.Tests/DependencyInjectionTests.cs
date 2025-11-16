using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TuBondi.Application.Extensions;
using TuBondi.Infrastructure.DependencyInjection;

namespace TuBondi.Tests;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void Resolves_transit_data_source_with_api_client_inside()
    {
        var services = new ServiceCollection();
        var settings = new Dictionary<string, string?>
        {
            ["TuBondi:BaseUrl"] = "https://micronauta4.dnsalias.net/",
            ["TuBondi:Conf"] = "cbaciudad"
        };
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        var provider = services.BuildServiceProvider();
        var dataSource = provider.GetRequiredService<ITransitDataSource>();
        dataSource.Should().BeOfType<TuBondiGateway>();
        var field = typeof(TuBondiGateway).GetField("_client", BindingFlags.Instance | BindingFlags.NonPublic);
        field.Should().NotBeNull();
        var client = field!.GetValue(dataSource);
        client.Should().BeAssignableTo<ITuBondiClient>();
    }
}
