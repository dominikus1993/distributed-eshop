using System.Text.Json.Serialization;
using Basket.Core.Model;
using Basket.Core.Requests;

using FastEndpoints;

using FluentValidation;

using Hosting.Extensions;

using Mediator;

namespace Basket.Api.Endpoints;

public sealed class RemoveItemFromCustomerBasketRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

[JsonSerializable(typeof(RemoveItemFromCustomerBasketRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class RemoveItemFromCustomerBasketEndpointCtx : JsonSerializerContext
{
    
}

public sealed class RemoveItemFromCustomerBasketRequestValidator : Validator<RemoveItemFromCustomerBasketRequest>
{
    public RemoveItemFromCustomerBasketRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Invalid product id");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("invalid quantity, should be grater than 0");
    }
}

public sealed class RemoveItemFromCustomerBasketEndpoint : Endpoint<RemoveItemFromCustomerBasketRequest>
{
    private readonly ISender _sender;

    public RemoveItemFromCustomerBasketEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure()
    {
        Delete("/api/Basket/items");
        SerializerContext(RemoveItemFromCustomerBasketEndpointCtx.Default);
        Validator<RemoveItemFromCustomerBasketRequestValidator>();
        Description(b => b
            .WithDescription("Remove or Decrease quantity of item in basket")
            .Accepts<RemoveItemFromCustomerBasketRequest>("application/json")
            .ProducesValidationProblem()
            .ProducesProblemFE(500)
            .Produces(200)
        );
        DontThrowIfValidationFails();
    }
    
    public override async Task HandleAsync(RemoveItemFromCustomerBasketRequest req, CancellationToken ct)
    {
        var userId = User.UserId();
        if (ValidationFailed)
        {
            await SendErrorsAsync(cancellation: ct);
            return;
        }
        
        await _sender.Send(new RemoveItemFromCustomerBasket(new CustomerId(userId), new Product(new ItemId(req.Id), new ItemQuantity((uint)req.Quantity))), ct);
        
        await SendOkAsync(cancellation: ct);
    }
}