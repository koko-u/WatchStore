using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using WatchStore.Api.Models.Dto;

namespace WatchStore.Api.OpenApiTransformers;

public sealed class PatchFieldSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        var type = context.JsonTypeInfo.Type;

        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(PatchField<>))
        {
            return Task.CompletedTask;
        }

        var valueType = type.GetGenericArguments()[0];

        var underlyingType = Nullable.GetUnderlyingType(valueType);
        var actualType = underlyingType ?? valueType;

        var includeNull = underlyingType is not null || !actualType.IsValueType; // 雑に参照型は nullable 扱い

        schema.Properties?.Clear();
        schema.Required?.Clear();

        if (actualType == typeof(string))
        {
            schema.Type = JsonSchemaType.String;
            schema.Format = null;
        }
        else if (actualType == typeof(int))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int32";
        }
        else if (actualType == typeof(long))
        {
            schema.Type = JsonSchemaType.Integer;
            schema.Format = "int64";
        }
        else if (actualType == typeof(decimal))
        {
            schema.Type = JsonSchemaType.Number;
            schema.Format = "decimal";
        }
        else if (actualType == typeof(bool))
        {
            schema.Type = JsonSchemaType.Boolean;
            schema.Format = null;
        }

        if (includeNull)
        {
            schema.Type |= JsonSchemaType.Null;
        }

        return Task.CompletedTask;
    }
}
