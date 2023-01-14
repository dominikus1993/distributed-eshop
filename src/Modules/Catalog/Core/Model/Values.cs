using StronglyTypedIds;

namespace Catalog.Core.Model;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.Guid, converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct ProductId
{
    public static ProductId From(Guid id) => new ProductId(id);
}