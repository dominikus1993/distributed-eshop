using System;

using Microsoft.Extensions.Logging;

using ShoppingList.Core.Dto;

namespace ShoppingList.Core.Logging;

internal static class Logger
{
    private static readonly Action<ILogger, AddItem, Exception?> _addingItemToCustomerBasket = LoggerMessage.Define<AddItem>(LogLevel.Debug, new EventId(1, nameof(AddItem)), "Adding item to shoppinglist, item: {Item}");

    public static void LogAddItem<T>(this ILogger<T> logger, AddItem item)
    {
        _addingItemToCustomerBasket(logger, item, null);
    }
}