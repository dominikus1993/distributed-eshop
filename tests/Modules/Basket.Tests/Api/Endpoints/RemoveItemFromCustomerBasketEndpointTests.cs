using Alba;

using Basket.Api.Endpoints;
using Basket.Tests.Extensions;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

[Collection(nameof(BasketApiFixtureCollectionTest))]
public sealed class RemoveItemFromCustomerBasketEndpointTests
{
        private readonly BasketApiFixture _basketApiFixture;

    public RemoveItemFromCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
    {
        _basketApiFixture = basketApiFixture;
    }
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasketAndRemoveIT_StatusReturnEmptyBasket()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await using var securityStub = JwtSecurityStubCreator.Create(customerId);

        await using var host = await _basketApiFixture.GetHost(securityStub);
        var itemid = Guid.NewGuid();
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 1, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await host.Scenario(s =>
        {
            s.Delete.Json(new RemoveItemFromCustomerBasketRequest() { Quantity = 1, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        // Act
        await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeNotFound();
        });
    }   
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasketAndAddItAgain_ShouldReturnBasketWithOneItemWithTwoElements()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await using var securityStub = JwtSecurityStubCreator.Create(customerId);

        await using var host = await _basketApiFixture.GetHost(securityStub);

        var itemid = Guid.NewGuid();
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 3, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await host.Scenario(s =>
        {
            s.Delete.Json(new RemoveItemFromCustomerBasketRequest() { Quantity = 1, Id = itemid }).ToUrl("/api/Basket/items");
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
        response.Items.ShouldContain(x => x.ItemId == itemid && x.Quantity == 2);
    }  
    
    [Fact]
    public async Task TestAddItemToEmptyCustomerBasketAndAddNewAgain_ShouldReturnBasketWithTwoItems()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        
        await using var securityStub = JwtSecurityStubCreator.Create(customerId);

        await using var host = await _basketApiFixture.GetHost(securityStub);

        var itemid = Guid.NewGuid();
        var itemid2 = Guid.NewGuid();
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 2, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await host.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 2, Id = itemid2 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });

        await host.Scenario(s =>
        {
            s.Delete.Json(new RemoveItemFromCustomerBasketRequest() { Quantity = 2, Id = itemid }).ToUrl("/api/Basket/items");
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
        response.Items.ShouldContain(x => x.ItemId == itemid2 && x.Quantity == 2);
    }  
}