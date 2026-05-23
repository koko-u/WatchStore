using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WatchStore.Api.Extensions;
using WatchStore.Api.Mappers;
using WatchStore.Api.Models.Domain;
using WatchStore.Api.Models.Dto;
using WatchStore.Api.Repositories;

namespace WatchStore.Api.Controllers;

[ApiController]
[Route("api/products")]
public sealed class ProductsController(ProductsRepository repo) : ControllerBase
{
    /// <summary>
    /// Get All Products
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Product>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Product>>> GetProducts(CancellationToken ct)
    {
        var rows = await repo.SelectAllAsync(ct);
        var products = rows.Select(r => r.ToModel());
        return Ok(products);
    }

    /// <summary>
    /// Get Product by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("id:int")]
    [ProducesResponseType<Product>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Product>> GetProductById(int id, CancellationToken ct)
    {
        var row = await repo.SelectByIdAsync(id, ct);
        if (row is null)
        {
            return Problem(title: $"Product of {id} is not found.", statusCode: StatusCodes.Status404NotFound);
        }

        return Ok(row.ToModel());
    }

    /// <summary>
    /// Create new Product
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="validator"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType<Product>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
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

        var row = await repo.InsertAsync(dto, ct);

        return CreatedAtAction(nameof(GetProductById), new { id = row.Id }, row.ToModel());
    }
}
