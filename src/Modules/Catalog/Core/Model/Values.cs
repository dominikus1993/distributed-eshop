using StronglyTypedIds;

namespace Catalog.Core.Model;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.Int, converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct ProductId
{
    
}