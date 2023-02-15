using StronglyTypedIds;

namespace Basket.Core.Model;

[StronglyTypedId(backingType: StronglyTypedIdBackingType.Guid, converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct ItemId
{
    
}