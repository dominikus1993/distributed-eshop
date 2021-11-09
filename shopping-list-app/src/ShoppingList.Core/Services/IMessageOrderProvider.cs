using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingList.Core.Model;

namespace ShoppingList.Core.Services
{
    public interface IShoppingListItemsOrderProvider
    {
        IAsyncEnumerable<Item> Sort(IEnumerable<Item> items);
    }

    public class DefaultShoppingListItemsOrderProvider : IShoppingListItemsOrderProvider
    {
        public async IAsyncEnumerable<Item> Sort(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                await Task.Delay(100);
                yield return item;
            }
        }
    }
}
