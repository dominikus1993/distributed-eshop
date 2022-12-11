using Basket.Core.Model;
using Basket.Core.Requests;

using FastEndpoints;

using Mediator;

namespace Basket.Api.Endpoints;

public sealed class GetCustomerBasketRequest
{
    
}

public sealed class GetCustomerBasketResponse
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
        Get("/");
    }

    public override async Task HandleAsync(CancellationToken c)
    {
        var result = await _sender.Send(new GetCustomerBasket(new CustomerId(Guid.NewGuid())), c);

        if (result is null)
        {
            await SendNotFoundAsync(c);
            return;
        }
        await SendAsync(new GetCustomerBasketResponse()
        {
            
        }, cancellation: c);
    }
}