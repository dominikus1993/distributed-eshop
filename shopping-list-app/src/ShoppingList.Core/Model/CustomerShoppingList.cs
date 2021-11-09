using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.ClassInstances;

namespace ShoppingList.Core.Model
{
    public readonly record struct ItemId(int Value);
    public readonly record struct CustomerId(int Value);
    public readonly record struct ItemQuantity(int Value)
    {
        public static ItemQuantity Zero() => new ItemQuantity(0);

        public static ItemQuantity Create(int q)
        {
            return new ItemQuantity(q);
        }

        public bool IsZero() => Value <= 0;

        public static ItemQuantity operator +(ItemQuantity item1, ItemQuantity item2)
        {
            return new ItemQuantity(item1.Value + item2.Value);
        }

        public static ItemQuantity operator -(ItemQuantity item1, ItemQuantity item2)
        {
            return new ItemQuantity(item1.Value - item2.Value);
        }
    }
    public class Item
    {
        public ItemId Id { get; }
        public ItemQuantity ProductQuantity { get; }

        public Item(ItemId id, ItemQuantity productQuantity)
        {
            Id = id;
            ProductQuantity = productQuantity;
        }

        public static Item operator +(Item item, ItemQuantity quantity)
        {
            return new Item(item.Id, item.ProductQuantity + quantity);
        }

        public static Item operator -(Item item1, ItemQuantity quantity)
        {
            if (quantity.IsZero())
            {
                return new Item(item1.Id, quantity);
            }
            return new Item(item1.Id, item1.ProductQuantity - quantity);
        }

        public static Item Empty(ItemId id)
        {
            return new Item(id, ItemQuantity.Zero());
        }

        public static Item Create(ItemId id, ItemQuantity quantity)
        {
            return new Item(id, quantity);
        }

        public static bool operator ==(Item a, Item b) => a.Id == b.Id;
        public static bool operator !=(Item a, Item b) => a.Id != b.Id;

        protected bool Equals(Item other)
        {
            return Id.Equals(other.Id);
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((Item)obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return $"{nameof(Id)}: {Id}, {nameof(ProductQuantity)}: {ProductQuantity}";
        }

        public bool HasItems()
        {
            return ProductQuantity.Value > 0;
        }

        public bool IsEmpty() => ProductQuantity.IsZero();
    }


    public class CustomerShoppingList
    {
        private readonly List<Item> _items;
        public IReadOnlyCollection<Item> Items => _items.AsReadOnly();

        public CustomerId CustomerId { get; }

        public bool IsEmpty() => _items.Count == 0;

        public CustomerShoppingList(CustomerId customerId,List<Item> items)
        {
            _items = items ?? new List<Item>();
            CustomerId = customerId;
        }

        public int TotalItemsQuantity() => _items.Sum(item => item.ProductQuantity.Value);

        public static CustomerShoppingList Empty(CustomerId customerId)
        {
            return new CustomerShoppingList(customerId, new List<Item>(0));
        }

        public void AddItem(Item basketItem)
        {
            var index = _items.IndexOf(basketItem);
            if (index != -1)
            {
                var item = _items[index];
                _items[index] = item + basketItem.ProductQuantity;
            }
            else
            {
                _items.Add(basketItem);
            }
        }

        public void Checkout()
        {
            if (IsEmpty())
            {
                return;
            }

            _items.Clear();
        }

        public void RemoveItem(Item basketItem)
        {
            var index = _items.IndexOf(basketItem);

            if (index == -1)
            {
                return;
            }

            var oldItem = _items[index];
            var newItem = oldItem - basketItem.ProductQuantity;

            if (newItem.IsEmpty())
            {
                _items.Remove(oldItem);
                return;
}

            if (newItem.HasItems())
            {
                _items[index] = newItem;
                return;
            }
        }

        public override string ToString()
        {
            return $"{nameof(_items)}: {string.Join(',', _items)}, {nameof(CustomerId)}: {CustomerId}";
        }
    }
}
