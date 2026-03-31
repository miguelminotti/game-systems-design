using System;

namespace REInventory.Core
{
    public class InventoryGridSlot : IInventoryGridSlot
    {
        public GridPosition GridPosition { get; }
        public IRuntimeStorable StoredItem { get; private set; }

        public event Action<IRuntimeStorable> OnItemStored;

        public InventoryGridSlot(GridPosition gridPosition)
        {
            GridPosition = gridPosition;
        }

        public InventoryGridSlot(int x, int y)
        {
            GridPosition = new GridPosition(x, y);
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