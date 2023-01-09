using Alba;
using Alba.Security;

using Basket.Core.Model;

using Microsoft.IdentityModel.JsonWebTokens;

namespace Basket.Tests.Extensions;

public static class AlbaExtensions
{
    public static Scenario StatusCodeShouldBeNotFound(this Scenario scenario)
    {
        scenario.StatusCodeShouldBe(404);
        return scenario;
    }
}

public static class JwtSecurityStubCreator
{
    public static JwtSecurityStub Create(CustomerId id)
    {
#pragma warning disable CA2000
        var jwt = new JwtSecurityStub();
#pragma warning restore CA2000
        return jwt
            .With(JwtRegisteredClaimNames.Sub, id.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");
    }
    
    public static JwtSecurityStub Create(Guid id)
    {
#pragma warning disable CA2000
        var jwt = new JwtSecurityStub();
#pragma warning restore CA2000
        return jwt
            .With(JwtRegisteredClaimNames.Sub, id.ToString())
            .With(JwtRegisteredClaimNames.UniqueName, "21372137")
            .With("iss", "test")
            .With("aud", "test")
            .WithName("janpawlacz2");
    }
}