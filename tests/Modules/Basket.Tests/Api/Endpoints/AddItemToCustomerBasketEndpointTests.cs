using Alba;
using Alba.Security;

using AutoFixture.Xunit2;

using Basket.Api.Endpoints;
using Basket.Tests.Extensions;
using Basket.Tests.Fixture;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

public sealed class AddItemToCustomerBasketEndpointTests : IClassFixture<BasketApiFixture>, IAsyncLifetime
{
    private readonly BasketApiFixture _basketApiFixture;
    private JwtSecurityStub _jwtSecurityStub;
    private IAlbaHost _albaHost;
    private Guid _customerId;
    
    public AddItemToCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
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
    public async Task TestAddItemToEmptyCustomerBasket_StatusReturnBasketWithOneItem(AddItemToCustomerBasketRequest request)
    {
        // Arrange
        var itemId = request.Id;
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(request).ToUrl("/api/Basket/items");
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
        response.Items.ShouldContain(x => x.ItemId == itemId && x.Quantity == request.Quantity);
    }   
    
    [Theory]
    [InlineAutoData]
    public async Task TestAddItemToEmptyCustomerBasketAndAddItAgain_ShouldReturnBasketWithOneIncreasedItem(AddItemToCustomerBasketRequest request)
    {
        // Arrange
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(request).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(request).ToUrl("/api/Basket/items");
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
        response.Items.ShouldContain(x => x.ItemId == request.Id);
    }  
    
    [Theory]
    [InlineAutoData]
    public async Task TestAddItemToEmptyCustomerBasketAndAddNewAgain_ShouldReturnBasketWithTwoItems(AddItemToCustomerBasketRequest item, AddItemToCustomerBasketRequest item2)
    {
        // Arrange
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(item).ToUrl("/api/Basket/items");
            s.StatusCodeShouldBeOk();
        });
        
        await _albaHost.Scenario(s =>
        {
            s.Post.Json(item2).ToUrl("/api/Basket/items");
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
        response.Items.Count.ShouldBe(2);
        response.Items.ShouldContain(x => x.ItemId == item.Id && x.Quantity == item.Quantity);
        response.Items.ShouldContain(x => x.ItemId == item2.Id && x.Quantity == item2.Quantity);
    }
}