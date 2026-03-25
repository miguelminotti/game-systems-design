using System;

namespace REInventory
{
    public interface IInventoryGridSlot
    {
        int X { get; }
        int Y { get; }
        IRuntimeStorable StoredItem { get; }
        event Action<IRuntimeStorable> OnItemStored;
        void StoreItem(IRuntimeStorable item);
        void RemoveItemStored();
        bool IsOccupied();
    }

    public class InventoryGridSlot : IInventoryGridSlot
    {
        public int X { get; }
        public int Y { get; }
        public IRuntimeStorable StoredItem { get; private set; }

        public event Action<IRuntimeStorable> OnItemStored;

        public InventoryGridSlot(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void StoreItem(IRuntimeStorable item)
        {
            StoredItem = item;
            OnItemStored?.Invoke(item);
        }

        public void RemoveItemStored()
        {
            if (StoredItem == null) return;
            StoredItem = null;
            OnItemStored?.Invoke(null);
        }

        public bool IsOccupied()
        {
            return StoredItem != null;
        }
    }
}