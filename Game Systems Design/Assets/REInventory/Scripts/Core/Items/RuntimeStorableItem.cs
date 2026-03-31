using MMStdLib.Utils;
using System;

namespace REInventory.Core
{
    // Testing class
    public class RuntimeStorableItem : IRuntimeStorable
    {
        // Properties
        public IStorable BaseItem { get; }
        public GridPosition StartingPosition { get; private set; }
        public GridPosition EndingPosition { get; private set; }
        public int Width => CurrentRotation == IRuntimeStorable.Rotation.Landscape ? BaseItem.Width : BaseItem.Height;
        public int Height => CurrentRotation == IRuntimeStorable.Rotation.Landscape ? BaseItem.Height : BaseItem.Width;
        public IRuntimeStorable.Rotation CurrentRotation { get; private set; } = IRuntimeStorable.Rotation.Landscape;
        public IStorableOption[] Options => new IStorableOption[]
        {
            new StorableOptionBase(Use, "Use"),
            new StorableOptionBase(Rotate, "Rotate"),
            new StorableOptionBase(Drop, "Drop"),
            new StorableOptionBase(Move, "Move"),
        };
        public IInventoryCore BindedInventory => _bindedInventory;

        // Private fields
        private IInventoryCore _bindedInventory;

        public event Action<GridPosition> OnPositionChanged;
        public event Action<IRuntimeStorable.Rotation> OnRotationChanged;

        public RuntimeStorableItem(IStorable baseItem)
        {
            BaseItem = baseItem;
        }

        public void SetPosition(GridPosition position)
        {
            StartingPosition = position;
            EndingPosition = position.Move(Width - 1, Height - 1);
            OnPositionChanged?.Invoke(position);
        }

        public void RotateTo(IRuntimeStorable.Rotation targetRotation)
        {
            if (CurrentRotation == targetRotation) return;
            CurrentRotation = targetRotation;
            OnRotationChanged?.Invoke(targetRotation);
        }

        private void Use()
        {
            // TODO: Add debug
        }

        private void Drop()
        {
            // TODO: Add debug
            _bindedInventory?.TryRemoveItem(this);
        }

        private void Move()
        {
            // TODO: Add debug
            if (_bindedInventory == null) return;
            if (_bindedInventory.TryRemoveItem(this))
            {
                IRuntimeStorable.IMoveInventoryItemEvent moveEvent = new MoveInventoryItemEvent(BindedInventory, this);
                GameEventBus.Publish(moveEvent);
            }
        }

        private void Rotate()
        {
            if (_bindedInventory.TryRotateItem(this))
            {
                if (CurrentRotation == IRuntimeStorable.Rotation.Landscape)
                {
                    RotateTo(IRuntimeStorable.Rotation.Portrait);
                    SetPosition(StartingPosition);
                }
                else
                {
                    RotateTo(IRuntimeStorable.Rotation.Landscape);
                    SetPosition(StartingPosition);
                }
            }
        }

        public void BindToInventory(IInventoryCore inventory)
        {
            _bindedInventory = inventory;
        }

        private class MoveInventoryItemEvent : IRuntimeStorable.IMoveInventoryItemEvent
        {
            public IInventoryCore RefInventory { get; }
            public IRuntimeStorable Item { get; }
            public MoveInventoryItemEvent(IInventoryCore refInventory, IRuntimeStorable item)
            {
                RefInventory = refInventory;
                Item = item;
            }
        }
    }
}