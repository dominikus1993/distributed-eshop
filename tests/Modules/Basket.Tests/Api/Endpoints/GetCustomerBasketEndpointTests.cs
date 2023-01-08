using Alba;
using Alba.Security;

using Basket.Core.Model;
using Basket.Infrastructure.Repositories;
using Basket.Infrastructure.Serialization;
using Basket.Tests.Fixture;

using Microsoft.IdentityModel.JsonWebTokens;

using Shouldly;

namespace Basket.Tests.Api.Endpoints;

public class GetCustomerBasketEndpointTests : IClassFixture<BasketApiFixture>
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
        await using var securityStub = new JwtSecurityStub()
            .With(JwtRegisteredClaimNames.Sub, customerId.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");

        await using var host = await _basketApiFixture.GetHost(securityStub);

        // Act
        var resp = await host.Scenario(s =>
        {
            s.Get.Url("/api/Basket");
            s.StatusCodeShouldBe(404);
        });
    }
}