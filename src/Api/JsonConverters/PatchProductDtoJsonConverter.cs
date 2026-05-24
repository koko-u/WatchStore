using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using WatchStore.Api.Models.Dto;

namespace WatchStore.Api.JsonConverters;

public sealed class PatchProductDtoJsonConverter : JsonConverter<PatchProductDto>
{
    public override PatchProductDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (root.ValueKind != JsonValueKind.Object)
        {
            throw new JsonException("Request body must be a JSON object.");
        }

        var dto = new PatchProductDto();

        if (root.TryGetProperty("name", out var nameElement))
        {
            dto.Name = nameElement.ValueKind switch
            {
                JsonValueKind.Null => PatchField<string?>.Specified(null),
                _ => PatchField<string?>.Specified(nameElement.GetString()!),
            };
        }
        else
        {
            dto.Name = PatchField<string?>.Unspecified;
        }

        if (root.TryGetProperty("description", out var descriptionElement))
        {
            dto.Description = descriptionElement.ValueKind switch
            {
                JsonValueKind.Null => PatchField<string?>.Specified(null),
                _ => PatchField<string?>.Specified(descriptionElement.GetString()),
            };
        }
        else
        {
            dto.Description = PatchField<string?>.Unspecified;
        }

        if (root.TryGetProperty("price", out var priceElement))
        {
            dto.Price = priceElement.ValueKind switch
            {
                JsonValueKind.Null => PatchField<decimal?>.Specified(null),
                _ => PatchField<decimal?>.Specified(priceElement.GetDecimal()),
            };
        }
        else
        {
            dto.Price = PatchField<decimal?>.Unspecified;
        }

        return dto;
    }

    public override void Write(Utf8JsonWriter writer, PatchProductDto value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Name.IsSpecified)
        {
            writer.WriteString("name", value.Name.Value);
        }

        if (value.Description.IsSpecified)
        {
            writer.WriteString("description", value.Description.Value);
            // writer.WritePropertyName("description");
            // JsonSerializer.Serialize(writer, value.Description.Value, options);
        }

        if (value.Price.IsSpecified)
        {
            if (value.Price.Value.HasValue)
            {
                writer.WriteNumber("price", value.Price.Value.Value);
            }
            else
            {
                writer.WriteNull("price");
            }
        }

        writer.WriteEndObject();
    }
}
