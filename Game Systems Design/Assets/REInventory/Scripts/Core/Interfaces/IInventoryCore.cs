using System.Collections.Generic;

namespace REInventory.Core
{
    public interface IInventoryCore
    {
        IReadOnlyList<IRuntimeStorable> Items { get; }
        int MaxCapacity { get; }
        int Width { get; }
        int Height { get; }
        void Initialize(IInventoryData data);
        bool TryAddItem(IRuntimeStorable item);
        IInventoryGrid.PlaceItemResult AddItemAtPosition(IRuntimeStorable item, GridPosition position);
        bool TryRemoveItem(IRuntimeStorable item);
        bool TryRotateItem(IRuntimeStorable item);
        void Clear();
    }
}