namespace WatchStore.Api.Models.Dto;

public sealed class NewProductDto
{
    /// <summary>
    /// 商品名です
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 商品説明です
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 価格です
    /// </summary>
    public decimal? Price { get; set; }
}
