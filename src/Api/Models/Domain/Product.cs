namespace WatchStore.Api.Models.Domain;

/// <summary>
/// 商品情報を保持するクラスです
/// </summary>
public sealed class Product
{
    /// <summary>
    /// ユニークIDです
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// 商品名です
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// 商品説明です
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 価格です
    /// </summary>
    public required decimal Price { get; set; }
}
