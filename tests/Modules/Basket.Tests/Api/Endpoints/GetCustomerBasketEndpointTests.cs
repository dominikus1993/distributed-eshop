using Alba;
using Alba.Security;

using AutoFixture.Xunit2;

using Basket.Core.Model;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Extensions;
using Basket.Tests.Fixture;

using Microsoft.IdentityModel.JsonWebTokens;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

[Collection(nameof(BasketApiFixtureCollectionTest))]
public class GetCustomerBasketEndpointTests: IAsyncLifetime
{
    private readonly BasketApiFixture _basketApiFixture;
    private JwtSecurityStub _jwtSecurityStub;
    private IAlbaHost _albaHost;
    private Guid _customerId;
    
    public GetCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
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
    public async Task TestWhenCustomerBasketNotExists_StatusCodeShouldBeNotFound()
    {
        // Act
        var resp = await _albaHost.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBe(404);
        });
    }
}