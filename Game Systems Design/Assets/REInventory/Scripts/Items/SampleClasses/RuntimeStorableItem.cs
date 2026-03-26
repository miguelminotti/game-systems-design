using MMStdLib.Utils;
using System;
using UnityEngine;

namespace REInventory
{
    public interface IMoveInventoryItemEvent
    {
        IInventoryCore RefInventory { get; }
        IRuntimeStorable Item { get; }
    }

    // Testing class
    public class RuntimeStorableItem : IRuntimeStorable
    {
        public IStorable BaseItem { get; }
        public int XPosition { get; private set; }
        public int YPosition { get; private set; }
        public IStorableOption[] Options => new IStorableOption[]
        {
            new StorableOptionBase(Use, "Use"),
            new StorableOptionBase(Drop, "Drop"),
            new StorableOptionBase(Move, "Move"),
        };
        public IInventoryCore BindedInventory => _bindedInventory;

        private IInventoryCore _bindedInventory;

        public event Action<int, int> OnPositionChanged;

        public RuntimeStorableItem(IStorable baseItem)
        {
            BaseItem = baseItem;
        }

        public void SetPosition(int x, int y)
        {
            XPosition = x;
            YPosition = y;
            OnPositionChanged?.Invoke(x, y);
        }

        private void Use()
        {
            Debug.Log($"Using item: {BaseItem}");
        }

        private void Drop()
        {
            Debug.Log($"Dropping item: {BaseItem}");
            _bindedInventory?.RemoveItem(this);
        }

        private void Move()
        {
            Debug.Log($"Moving item: {BaseItem}");
            _bindedInventory?.RemoveItem(this);
            IMoveInventoryItemEvent moveEvent = new MoveInventoryItemEvent(BindedInventory, this);
            GameEventBus.Publish(moveEvent);
        }

        public void BindToInventory(IInventoryCore inventory)
        {
            _bindedInventory = inventory;
        }

        private class MoveInventoryItemEvent : IMoveInventoryItemEvent
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