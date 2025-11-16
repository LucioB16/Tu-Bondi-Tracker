namespace TuBondi.Tests;

public sealed class ApplicationDecouplingTests
{
    [Fact]
    public void Application_layer_has_no_api_client_reference()
    {
        var references = typeof(ITransitDataSource).Assembly.GetReferencedAssemblies();
        references.Should().NotContain(r => r.Name.Contains("TuBondi.ApiClient", StringComparison.OrdinalIgnoreCase));
    }
}
