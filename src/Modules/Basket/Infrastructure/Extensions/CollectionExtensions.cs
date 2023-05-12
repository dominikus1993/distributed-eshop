namespace Basket.Infrastructure.Extensions;

public static class CollectionExtensions
{
    public static IReadOnlyList<TDest> Map<TSrc, TDest>(this IReadOnlyList<TSrc>? src, Func<TSrc, TDest> map)
    {
        if (src is null or {Count: 0})
        {
            return Array.Empty<TDest>();
        }

        if (src is [var element])
        {
            return new[] { map(element) };
        }
        
        var array = new TDest[src.Count];

        for (int i = 0; i < src.Count; i++)
        {
            array[i] = map(src[i]);
        }

        return array;
    }
}