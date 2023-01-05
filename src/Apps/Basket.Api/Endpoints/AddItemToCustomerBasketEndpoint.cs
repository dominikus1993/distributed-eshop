using System.Text.Json.Serialization;

using Basket.Core.Model;
using Basket.Core.Requests;

using FastEndpoints;

using FluentValidation;

using Hosting.Extensions;

using Mediator;

namespace Basket.Api.Endpoints;

public sealed class AddItemToCustomerBasketRequest
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

[JsonSerializable(typeof(AddItemToCustomerBasketRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
public partial class AddItemToCustomerBasketEndpointCtx : JsonSerializerContext
{
    
}


public sealed class AddItemToBasketRequestValidator : Validator<AddItemToCustomerBasketRequest>
{
    public AddItemToBasketRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().NotNull().WithMessage("Invalid product id");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("invalid quantity, should be grater than 0");
    }
}


public sealed class AddItemToCustomerBasketEndpoint : Endpoint<AddItemToCustomerBasketRequest>
{
    private readonly ISender _sender;

    public AddItemToCustomerBasketEndpoint(ISender sender)
    {
        _sender = sender;
    }
    
    public override void Configure()
    {
        Post("/api/Basket/items");
        SerializerContext(AddItemToCustomerBasketEndpointCtx.Default);
        Validator<AddItemToBasketRequestValidator>();
        Description(b => b
            .WithDescription("Add item to basket")
            .Accepts<AddItemToCustomerBasketRequest>("application/json")
            .ProducesProblemFE(500)
            .ProducesValidationProblem()
            .Produces(200)
        );
        DontThrowIfValidationFails();
    }
    
    public override async Task HandleAsync(AddItemToCustomerBasketRequest r, CancellationToken c)
    {
        var userId = User.UserId();
        if (ValidationFailed)
        {
            base.Logger.LogWarning("Invalid request");
            await SendErrorsAsync(cancellation: c);
            return;
        }
        
        await _sender.Send(new AddItemToCustomerBasket(new CustomerId(userId), new BasketItem(new ItemId(r.Id), new ItemQuantity((uint)r.Quantity))), c);
        
        await SendOkAsync(cancellation: c);
    }
    
}