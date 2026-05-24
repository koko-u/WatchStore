using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using WatchStore.Api.Models.Dto;

namespace WatchStore.Api.OpenApiTransformers;

public sealed class PatchProductDtoSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(OpenApiSchema schema, OpenApiSchemaTransformerContext context, CancellationToken ct)
    {
        if (context.JsonTypeInfo.Type != typeof(PatchProductDto))
        {
            return Task.CompletedTask;
        }

        schema.Type = JsonSchemaType.Object;
        schema.Required?.Clear();

        schema.Properties ??= new Dictionary<string, IOpenApiSchema>();
        schema.Properties.Clear();

        schema.Properties["name"] = new OpenApiSchema
        {
            Type = JsonSchemaType.String,
            MaxLength = 255,
            MinLength = 1,
        };

        schema.Properties["description"] = new OpenApiSchema
        {
            Type = JsonSchemaType.String | JsonSchemaType.Null,
            MaxLength = 4000,
        };

        schema.Properties["price"] = new OpenApiSchema
        {
            Type = JsonSchemaType.Number,
            Format = "double",
            Minimum = "0",
        };

        schema.Required ??= new HashSet<string>();
        schema.Required.Add("name");
        schema.Required.Add("price");

        return Task.CompletedTask;
    }
}
