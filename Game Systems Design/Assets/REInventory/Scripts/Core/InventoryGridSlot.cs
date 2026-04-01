namespace REInventory.Core
{
    public class InventoryGridSlot : IInventoryGridSlot
    {
        public GridPosition GridPosition { get; }
        public IRuntimeStorable StoredItem { get; private set; }

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
        }

        public void RemoveItemStored()
        {
            if (StoredItem == null) return;
            StoredItem = null;
        }

        public bool IsOccupied()
        {
            return StoredItem != null;
        }
    }
}