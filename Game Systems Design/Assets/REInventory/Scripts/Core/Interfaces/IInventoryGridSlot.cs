namespace REInventory.Core
{
    public interface IInventoryGridSlot
    {
        GridPosition GridPosition { get; }
        IRuntimeStorable StoredItem { get; }
        void StoreItem(IRuntimeStorable item);
        void RemoveItemStored();
        bool IsOccupied();
    }
}