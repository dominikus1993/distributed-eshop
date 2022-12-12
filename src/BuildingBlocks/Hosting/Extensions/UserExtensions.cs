using System.Security.Claims;

namespace Hosting.Extensions;

public static class UserExtensions
{
    public static Guid UserId(this ClaimsPrincipal user)
    {
        if (Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException();
    }
}