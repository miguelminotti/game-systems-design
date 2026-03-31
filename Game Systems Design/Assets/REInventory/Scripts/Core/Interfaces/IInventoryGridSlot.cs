using System;

namespace REInventory.Core
{
    public interface IInventoryGridSlot
    {
        GridPosition GridPosition { get; }
        IRuntimeStorable StoredItem { get; }
        event Action<IRuntimeStorable> OnItemStored;
        void StoreItem(IRuntimeStorable item);
        void RemoveItemStored();
        bool IsOccupied();
    }
}