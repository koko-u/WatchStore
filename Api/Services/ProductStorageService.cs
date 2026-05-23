using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AutoRegisterAnnotation;
using WatchStore.Api.Models.Domain;

namespace WatchStore.Api.Services;

[AutoRegisterService]
public sealed class ProductStorageService
{
    private readonly string _jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "products.json");
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<List<Product>> ReadProductsAsync(CancellationToken ct)
    {
        if (!File.Exists(_jsonPath))
        {
            throw new FileNotFoundException("Products file not found", _jsonPath);
        }

        await using var json = File.OpenRead(_jsonPath);

        var products = await JsonSerializer.DeserializeAsync<List<Product>>(json, _jsonOptions, ct);
        if (products is null)
        {
            throw new JsonException("Failed to deserialize products from JSON");
        }

        return products;
    }

    public async Task SaveProductsAsync(List<Product> products, CancellationToken ct)
    {
        await _gate.WaitAsync(ct);
        try
        {
            var tempPath = _jsonPath + ".tmp";
            await using var stream = File.Create(tempPath);
            await JsonSerializer.SerializeAsync(stream, products, _jsonOptions, ct);

            File.Move(tempPath, _jsonPath, overwrite: true);
        }
        finally
        {
            _gate.Release();
        }
    }
}
