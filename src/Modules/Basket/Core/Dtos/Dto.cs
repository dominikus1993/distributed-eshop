using System.Diagnostics.CodeAnalysis;

using Basket.Core.Model;

namespace Basket.Core.Dtos;

public sealed class CustomerBasketDto
{
    public required CustomerId CustomerId { get; init; }

    public CustomerBasketDto()
    {
        
    }
    
    [SetsRequiredMembers]
    public CustomerBasketDto(CustomerBasket basket)
    {
        CustomerId = basket.CustomerId;
        
    }
}