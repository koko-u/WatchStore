using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using WatchStore.Api.Models.Domain;
using WatchStore.Api.Models.Dto;

namespace WatchStore.Api.Tests;

[Collection("ApiTests")]
public sealed class ProductsTests(ApiFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    private readonly NpgsqlDataSource _dataSource = factory.Services.GetRequiredService<NpgsqlDataSource>();

    private CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Get_Products_ReturnOk()
    {
        var response = await _client.GetAsync("/api/products", CancellationToken);

        await using var body = await response.Content.ReadAsStreamAsync(CancellationToken);
        var products = await JsonSerializer.DeserializeAsync<List<Product>>(
            body,
            JsonSerializerOptions.Default,
            CancellationToken
        );

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue();
            products.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task Get_ProductById_ReturnOk()
    {
        await using var conn = await _dataSource.OpenConnectionAsync(CancellationToken);
        await using var tx = await conn.BeginTransactionAsync(CancellationToken);
        var cmd = new CommandDefinition(
            commandText: """
            INSERT INTO "products" ("name", "description", "price")
            VALUES ('One', 'Product one', 100),
                   ('Two', 'Product two', 200)
            RETURNING "id";
            """,
            transaction: tx,
            cancellationToken: CancellationToken
        );
        var ids = (await conn.QueryAsync<int>(cmd)).ToList();
        await tx.CommitAsync(CancellationToken);

        var response = await _client.GetAsync($"/api/products/{ids[0]}", CancellationToken);
        //var body = await response.Content.ReadAsStringAsync(CancellationToken);
        await using var body = await response.Content.ReadAsStreamAsync(CancellationToken);
        var product = await JsonSerializer.DeserializeAsync<Product>(
            body,
            JsonSerializerOptions.Web,
            CancellationToken
        );

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue("Status Code = {0}", response.StatusCode);
            product.Should().NotBeNull();
            product
                .Should()
                .BeEquivalentTo(
                    new Product
                    {
                        Id = ids[0],
                        Name = "One",
                        Description = "Product one",
                        Price = 100,
                    }
                );
        }
    }

    [Fact]
    public async Task Post_Product_Returns_Created()
    {
        var dto = new NewProductDto
        {
            Name = "Three",
            Description = "Product 3",
            Price = 300,
        };
        var response = await _client.PostAsJsonAsync($"/api/products", dto, CancellationToken);
        await using var body = await response.Content.ReadAsStreamAsync(CancellationToken);
        var product = await JsonSerializer.DeserializeAsync<Product>(
            body,
            JsonSerializerOptions.Web,
            CancellationToken
        );

        using (new AssertionScope())
        {
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            product.Should().NotBeNull();
            product
                .Should()
                .BeEquivalentTo(
                    new
                    {
                        Name = "Three",
                        Description = "Product 3",
                        Price = 300,
                    }
                );
        }
    }

    [Fact]
    public async Task Patch_Product_ReturnOk()
    {
        await using var conn = await _dataSource.OpenConnectionAsync(CancellationToken);
        await using var tx = await conn.BeginTransactionAsync(CancellationToken);
        var cmd = new CommandDefinition(
            commandText: """
            INSERT INTO "products" ("name", "description", "price")
            VALUES ('One', 'Product one', 100),
                   ('Two', 'Product two', 200)
            RETURNING "id";
            """,
            transaction: tx,
            cancellationToken: CancellationToken
        );
        var ids = (await conn.QueryAsync<int>(cmd)).ToList();
        await tx.CommitAsync(CancellationToken);

        // name is unchanged
        // description is set null
        // price is set to 500
        var dto = new PatchProductDto
        {
            Name = PatchField<string?>.Unspecified,
            Description = PatchField<string?>.Specified(null),
            Price = PatchField<decimal?>.Specified(500),
        };
        var response = await _client.PatchAsJsonAsync($"/api/products/{ids[0]}", dto, CancellationToken);
        await using var body = await response.Content.ReadAsStreamAsync(CancellationToken);
        var product = await JsonSerializer.DeserializeAsync<Product>(
            body,
            JsonSerializerOptions.Web,
            CancellationToken
        );

        using (new AssertionScope())
        {
            response.IsSuccessStatusCode.Should().BeTrue("Status Code = {0}", response.StatusCode);
            product.Should().NotBeNull();
            product
                .Should()
                .BeEquivalentTo(
                    new Product
                    {
                        Id = ids[0],
                        Name = "One",
                        Description = null,
                        Price = 500,
                    }
                );
        }
    }

    public async ValueTask InitializeAsync() => await ResetDatabaseAsync();

    public async ValueTask DisposeAsync() => await ResetDatabaseAsync();

    private async Task ResetDatabaseAsync()
    {
        await using var conn = await _dataSource.OpenConnectionAsync(CancellationToken);
        await using var tx = await conn.BeginTransactionAsync(CancellationToken);
        try
        {
            var cmd = new CommandDefinition(
                commandText: """
                TRUNCATE TABLE
                    products
                RESTART IDENTITY CASCADE;
                """,
                transaction: tx,
                cancellationToken: CancellationToken
            );
            await conn.ExecuteAsync(cmd);

            await tx.CommitAsync(CancellationToken);
        }
        catch (Exception)
        {
            await tx.RollbackAsync(CancellationToken);
            throw;
        }
    }
}
