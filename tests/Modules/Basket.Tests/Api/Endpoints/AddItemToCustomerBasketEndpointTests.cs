using Alba;
using Alba.Security;

using Basket.Api.Endpoints;
using Basket.Tests.Fixture;

using Microsoft.IdentityModel.JsonWebTokens;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

public class AddItemToCustomerBasketEndpointTests : IClassFixture<BasketApiFixture>
{
    private readonly BasketApiFixture _basketApiFixture;

    public AddItemToCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
    {
        _basketApiFixture = basketApiFixture;
    }
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasket_StatusReturnBasketWithOneItem()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var securityStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Sub, customerId.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");

        await using var host = await _basketApiFixture.GetHost(securityStub);

        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 1, Id = 1 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        
        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeOk();
        });

        var response = await resp.ReadAsJsonAsync<GetCustomerBasketResponse>();
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(customerId);
        response.Items.ShouldNotBeEmpty();
        response.Items.Count.ShouldBe(1);
        response.Items.ShouldContain(x => x.ItemId == 1 && x.Quantity == 1);
    }   
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasketAndAddItAgain_ShouldReturnBasketWithOneIncreasedItem()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var securityStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Sub, customerId.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");

        await using var host = await _basketApiFixture.GetHost(securityStub);

        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 1, Id = 1 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 1, Id = 1 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });

        
        
        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeOk();
        });

        var response = await resp.ReadAsJsonAsync<GetCustomerBasketResponse>();
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(customerId);
        response.Items.ShouldNotBeEmpty();
        response.Items.Count.ShouldBe(1);
        response.Items.ShouldContain(x => x.ItemId == 1 && x.Quantity == 2);
    }  
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasketAndAddNewAgain_ShouldReturnBasketWithTwoItems()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var securityStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Sub, customerId.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");

        await using var host = await _basketApiFixture.GetHost(securityStub);

        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 1, Id = 1 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 2, Id = 2 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });

        
        
        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeOk();
        });

        var response = await resp.ReadAsJsonAsync<GetCustomerBasketResponse>();
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(customerId);
        response.Items.ShouldNotBeEmpty();
        response.Items.Count.ShouldBe(2);
        response.Items.ShouldContain(x => x.ItemId == 1 && x.Quantity == 1);
        response.Items.ShouldContain(x => x.ItemId == 2 && x.Quantity == 2);
    }  
}