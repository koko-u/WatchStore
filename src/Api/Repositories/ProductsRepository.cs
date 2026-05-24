using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoRegisterAnnotation;
using Dapper;
using KozLibraries.DapperSqlHelper;
using Microsoft.Extensions.Logging;
using Npgsql;
using WatchStore.Api.Extensions;
using WatchStore.Api.Models.Dto;
using WatchStore.Api.Models.Rows;

namespace WatchStore.Api.Repositories;

[AutoRegisterService]
public sealed class ProductsRepository(NpgsqlDataSource dataSource, SqlResource sql, ILogger<ProductsRepository> logger)
{
    /// <summary>
    /// Select all products from the database
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ProductRow>> SelectAllAsync(CancellationToken ct)
    {
        var selectSql = await sql.GetAsync("Products/select_all.sql", ct);
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var selectCmd = new CommandDefinition(commandText: selectSql, cancellationToken: ct);
        var rows = await conn.QueryAsync<ProductRow>(selectCmd);

        return rows;
    }

    /// <summary>
    /// Select product by id from the database
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<ProductRow?> SelectByIdAsync(int id, CancellationToken ct)
    {
        var selectSql = await sql.GetAsync("Products/select_by_id.sql", ct);
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        var selectCmd = new CommandDefinition(
            commandText: selectSql,
            parameters: new { Id = id },
            cancellationToken: ct
        );
        var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(selectCmd);

        return row;
    }

    /// <summary>
    /// Insert a new product into the database
    /// </summary>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public async Task<ProductRow> InsertAsync(NewProductDto dto, CancellationToken ct)
    {
        var insertSql = await sql.GetAsync("Products/insert_one.sql", ct);
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        try
        {
            var insertCmd = new CommandDefinition(
                commandText: insertSql,
                parameters: new
                {
                    dto.Name,
                    dto.Description,
                    dto.Price,
                },
                transaction: tx,
                cancellationToken: ct
            );
            var row = await conn.QuerySingleAsync<ProductRow>(insertCmd);

            await tx.CommitAsync(ct);

            return row;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to insert product with name '{Name}'", dto.Name);
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    /// <summary>
    /// Partial update of a product by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="dto"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<ProductRow?> UpdateAsync(int id, PatchProductDto dto, CancellationToken ct)
    {
        var patchSql = await sql.GetAsync("Products/patch_template.sql", ct);
        var builder = new SqlBuilder();
        var template = builder.AddTemplate(patchSql);
        builder.AddParameters(new { Id = id });

        var setName = builder.SetIfSpecified("name = @Name", () => dto.Name);
        var setDesc = builder.SetIfSpecified("description = @Description", () => dto.Description);
        var setPrice = builder.SetIfSpecified("price = @Price", () => dto.Price);
        if (!setName && !setDesc && !setPrice)
        {
            throw new InvalidOperationException("No fields specified for update");
        }

        await using var conn = await dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        try
        {
            var patchCmd = new CommandDefinition(
                commandText: template.RawSql,
                parameters: template.Parameters,
                transaction: tx,
                cancellationToken: ct
            );
            var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(patchCmd);

            await tx.CommitAsync(ct);

            return row;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to update product with id '{id}'", id);
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    public async Task<ProductRow?> DeleteByIdAsync(int id, CancellationToken ct)
    {
        var deleteSql = await sql.GetAsync("Products/delete_by_id.sql", ct);
        await using var conn = await dataSource.OpenConnectionAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        try
        {
            var deleteCmd = new CommandDefinition(
                commandText: deleteSql,
                parameters: new { Id = id },
                transaction: tx,
                cancellationToken: ct
            );
            var row = await conn.QuerySingleOrDefaultAsync<ProductRow>(deleteCmd);

            await tx.CommitAsync(ct);

            return row;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete product with id '{id}'", id);
            await tx.RollbackAsync(ct);
            throw;
        }
    }
}
