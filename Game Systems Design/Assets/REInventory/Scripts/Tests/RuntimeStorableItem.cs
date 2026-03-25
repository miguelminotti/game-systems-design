using System;
using UnityEngine;

namespace REInventory
{
    // Testing class
    public class RuntimeStorableItem : IRuntimeStorable
    {
        public IStorable BaseItem { get; }
        public int XPosition { get; private set; }
        public int YPosition { get; private set; }
        public IStorableOption[] Options => new IStorableOption[]
        {
            new StorableOptionUse(Use),
            new StorableOptionDrop(Drop),
            new StorableOptionMove(Move),
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
            _bindedInventory?.RemoveItem(this);
        }

        private void Move()
        {

        }

        public void BindToInventory(IInventoryCore inventory)
        {
            _bindedInventory = inventory;
        }
    }
}