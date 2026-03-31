using System;

namespace REInventory.Core
{
    public interface IRuntimeStorable
    {
        enum Rotation
        {
            Landscape,
            Portrait,
        }

        IStorable BaseItem { get; }
        GridPosition StartingPosition { get; }
        GridPosition EndingPosition { get; }
        int Width { get; }
        int Height { get; }
        Rotation CurrentRotation { get; }
        IStorableOption[] Options { get; }
        IInventoryCore BindedInventory { get; }

        event Action<GridPosition> OnPositionChanged;
        event Action<Rotation> OnRotationChanged;

        void SetPosition(GridPosition position);
        void BindToInventory(IInventoryCore inventory);
        void RotateTo(Rotation targetRotation);

        interface IMoveInventoryItemEvent
        {
            IInventoryCore RefInventory { get; }
            IRuntimeStorable Item { get; }
        }
    }
}