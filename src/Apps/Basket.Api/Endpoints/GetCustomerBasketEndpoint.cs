using System.Text.Json.Serialization;

using Basket.Core.Dtos;
using Basket.Core.Model;
using Basket.Core.Requests;

using FastEndpoints;

using Hosting.Extensions;

using Mediator;

namespace Basket.Api.Endpoints;

public sealed class BasketItemResponseDto
{
    public Guid ItemId { get; set; }
    public uint Quantity { get; set; }

    public BasketItemResponseDto()
    {
        
    }
    
    public BasketItemResponseDto(BasketItemDto dto)
    {
        ItemId = dto.ItemId;
        Quantity = dto.Quantity;
    }
}

public sealed class GetCustomerBasketResponse
{
    public Guid CustomerId { get; set; }
    public IReadOnlyCollection<BasketItemResponseDto> Items { get; set; } = null!;

    public GetCustomerBasketResponse()
    {
        
    }
    public GetCustomerBasketResponse(CustomerBasketDto basket)
    {
        CustomerId = basket.CustomerId;
        Items = basket.Items.Select(item => new BasketItemResponseDto(item)).ToArray();
    }
}

[JsonSerializable(typeof(GetCustomerBasketResponse))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class GetCustomerBasketEndpointCtx : JsonSerializerContext
{
    
}

public sealed class GetCustomerBasketEndpoint : EndpointWithoutRequest<GetCustomerBasketResponse>
{
    private readonly ISender _sender;

    public GetCustomerBasketEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure()
    {
        Get("/api/Basket");
        SerializerContext(GetCustomerBasketEndpointCtx.Default);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.UserId();
        var result = await _sender.Send(new GetCustomerBasket(new CustomerId(userId)), ct);

        if (result is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        await SendAsync(new GetCustomerBasketResponse(result), cancellation: ct);
    }
}