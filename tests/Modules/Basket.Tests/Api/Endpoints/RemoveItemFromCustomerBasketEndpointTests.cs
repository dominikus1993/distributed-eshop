using Alba;
using Alba.Security;

using AutoFixture.Xunit2;

using Basket.Api.Endpoints;
using Basket.Tests.Extensions;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

[Collection(nameof(BasketApiFixtureCollectionTest))]
public sealed class RemoveItemFromCustomerBasketEndpointTests : IAsyncLifetime
{
    private readonly BasketApiFixture _basketApiFixture;
    private JwtSecurityStub _jwtSecurityStub;
    private IAlbaHost _albaHost;
    private readonly Guid _customerId;
    
    public RemoveItemFromCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
    {
        _basketApiFixture = basketApiFixture;
        _customerId = Guid.NewGuid();
    }
    
    public async Task InitializeAsync()
    {
        _jwtSecurityStub = JwtSecurityStubCreator.Create(_customerId);
        _albaHost = await _basketApiFixture.GetHost(_jwtSecurityStub);
    }

    public async Task DisposeAsync()
    {
        await ((IAsyncDisposable)_jwtSecurityStub).DisposeAsync();
        await _albaHost.DisposeAsync();
    }
    
    [Theory]
    [InlineAutoData]
    public async Task TestAddItemToEmptyCustomerBasketAndRemoveIT_StatusReturnEmptyBasket(AddItemToCustomerBasketRequest request)
    {
        // Arrange
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(request).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await _albaHost.Scenario(s =>
        {
            s.Delete.Json(request).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        // Act
        await _albaHost.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeNotFound();
        });
    }   
    
    
    [Theory]
    [InlineAutoData]
    public async Task TestAddItemToEmptyCustomerBasketAndAddItAgain_ShouldReturnBasketWithOneItemWithTwoElements(Guid itemid)
    {
        // Arrange
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 3, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await _albaHost.Scenario(s =>
        {
            s.Delete.Json(new RemoveItemFromCustomerBasketRequest() { Quantity = 1, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        // Act
        var resp = await _albaHost.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeOk();
        });

        var response = await resp.ReadAsJsonAsync<GetCustomerBasketResponse>();
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(_customerId);
        response.Items.ShouldNotBeEmpty();
        response.Items.Count.ShouldBe(1);
        response.Items.ShouldContain(x => x.ItemId == itemid && x.Quantity == 2);
    }  
    
    [Theory]
    [InlineAutoData]
    public async Task TestAddItemToEmptyCustomerBasketAndAddNewAgain_ShouldReturnBasketWithTwoItems(Guid itemid, Guid itemid2)
    {
        // Arrange
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 2, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(new AddItemToCustomerBasketRequest() { Quantity = 2, Id = itemid2 }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });

        await _albaHost.Scenario(s =>
        {
            s.Delete.Json(new RemoveItemFromCustomerBasketRequest() { Quantity = 2, Id = itemid }).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        // Act
        var resp = await _albaHost.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBeOk();
        });

        var response = await resp.ReadAsJsonAsync<GetCustomerBasketResponse>();
        response.ShouldNotBeNull();
        response.CustomerId.ShouldBe(_customerId);
        response.Items.ShouldNotBeEmpty();
        response.Items.Count.ShouldBe(1);
        response.Items.ShouldContain(x => x.ItemId == itemid2 && x.Quantity == 2);
    }  
}