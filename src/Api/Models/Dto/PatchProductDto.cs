using System.Text.Json.Serialization;
using WatchStore.Api.JsonConverters;

namespace WatchStore.Api.Models.Dto;

[JsonConverter(typeof(PatchProductDtoJsonConverter))]
public sealed class PatchProductDto
{
    /// <summary>
    /// 商品名です
    /// </summary>
    public PatchField<string?> Name { get; set; }

    /// <summary>
    /// 商品説明です
    /// </summary>
    public PatchField<string?> Description { get; set; }

    /// <summary>
    /// 価格です
    /// </summary>
    public PatchField<decimal?> Price { get; set; }
}
