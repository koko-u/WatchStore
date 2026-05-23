using Riok.Mapperly.Abstractions;
using WatchStore.Api.Models.Domain;
using WatchStore.Api.Models.Rows;

namespace WatchStore.Api.Mappers;

[Mapper(ThrowOnPropertyMappingNullMismatch = true)]
public static partial class ProductMapper
{
    public static partial Product ToModel(this ProductRow row);
}
