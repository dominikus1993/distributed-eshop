using StronglyTypedIds;

namespace Basket.Core.Model;

[StronglyTypedId(converters: StronglyTypedIdConverter.SystemTextJson)]
public readonly partial struct CustomerId
{
    
}