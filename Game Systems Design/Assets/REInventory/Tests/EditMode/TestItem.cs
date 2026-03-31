using REInventory.Core;
using System;

namespace REInventory.Tests
{
    public class TestItem : IRuntimeStorable
    {
        public IStorable BaseItem => null;
        public GridPosition StartingPosition { get; private set; }
        public GridPosition EndingPosition { get; private set; }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public IRuntimeStorable.Rotation CurrentRotation { get; private set; }
        public IStorableOption[] Options { get; private set; }
        public IInventoryCore BindedInventory { get; private set; }

        public event Action<GridPosition> OnPositionChanged;
        public event Action<IRuntimeStorable.Rotation> OnRotationChanged;

        public TestItem(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public void BindToInventory(IInventoryCore inventory)
        {

        }

        public void RotateTo(IRuntimeStorable.Rotation targetRotation)
        {

        }

        public void SetPosition(GridPosition position)
        {
            StartingPosition = position;
            EndingPosition = position.Move(Width - 1, Height - 1);
        }
    }
}