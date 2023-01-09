using Alba;
using Alba.Security;

using Basket.Core.Model;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Extensions;
using Basket.Tests.Fixture;

using Microsoft.IdentityModel.JsonWebTokens;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

[Collection(nameof(BasketApiFixtureCollectionTest))]
public class GetCustomerBasketEndpointTests
{
    private BasketApiFixture _basketApiFixture;

    public GetCustomerBasketEndpointTests(BasketApiFixture basketApiFixture)
    {
        _basketApiFixture = basketApiFixture;
    }
    
    [Fact]
    public async Task TestWhenCustomerBasketNotExists_StatusCodeShouldBeNotFound()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        await using var jwtSecurity = new JwtSecurityStub();
        await using var securityStub = JwtSecurityStubCreator.Create(customerId);

        await using var host = await _basketApiFixture.GetHost(securityStub);

        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBe(404);
        });
    }
}