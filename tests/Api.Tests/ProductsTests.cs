using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace WatchStore.Api.Tests;

[Collection("ApiTests")]
public sealed class ProductsTests(ApiFactory factory)
{
    //private readonly HttpClient _client = factory.CreateClient();
    private CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Get_Products_ReturnOk()
    {
        using var client = factory.CreateClient();
        var response = await client.GetAsync("/api/products", CancellationToken);

        var body = await response.Content.ReadAsStringAsync(CancellationToken);

        response.IsSuccessStatusCode.Should().BeTrue(body);
    }
}
