using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WatchStore.Api.Tests;

[Collection("ApiTests")]
public sealed class HealthTests(ApiFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Get_Health_ReturnOk()
    {
        var response = await _client.GetAsync("/health", CancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
