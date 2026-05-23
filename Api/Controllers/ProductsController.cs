using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using WatchStore.Api.Extensions;
using WatchStore.Api.Models.Domain;
using WatchStore.Api.Models.Dto;
using WatchStore.Api.Services;

namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ProductStorageService productStorage) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetProductsAsync(CancellationToken ct)
    {
        var products = await productStorage.ReadProductsAsync(ct);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult> CreatePost(
        [FromBody] NewProductDto dto,
        [FromServices] IValidator<NewProductDto> validator,
        CancellationToken ct
    )
    {
        var result = await validator.ValidateAsync(dto, ct);
        if (!result.IsValid)
        {
            return ValidationProblem(result.IntoProblemDetails());
        }

        //
        var products = await productStorage.ReadProductsAsync(ct);
        var maxId = products.Select(p => p.Id).MaxOrElse(0);
        products.Add(
            new Product
            {
                Id = maxId + 1,
                Name = dto.Name!,
                Description = dto.Description,
                Price = dto.Price!.Value,
            }
        );

        await productStorage.SaveProductsAsync(products, ct);
        return Created();
    }
}
