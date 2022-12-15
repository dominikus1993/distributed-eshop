using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

using Basket.Core.Model;

using StackExchange.Redis;

namespace Basket.Infrastructure.Extensions;

internal static class RedisExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static RedisKey ToRedisKey(this CustomerId id) => new($"CustomerBasket:{id.Value.ToString()}");
        
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGetValue(this RedisValue value, [NotNullWhen(true)]out string? result)
    {
        if (value.IsNullOrEmpty)
        {
            result = null;
            return false;
        }
        result = value!;
        return true;
    }
}