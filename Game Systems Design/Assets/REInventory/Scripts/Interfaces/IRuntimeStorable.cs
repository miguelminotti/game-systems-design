using System;

namespace REInventory
{
    public interface IRuntimeStorable
    {
        IStorable BaseItem { get; }
        int XPosition { get; }
        int YPosition { get; }
        IStorableOption[] Options { get; }
        IInventoryCore BindedInventory { get; }

        event Action<int, int> OnPositionChanged;

        void SetPosition(int x, int y);
        void BindToInventory(IInventoryCore inventory);
    }
}