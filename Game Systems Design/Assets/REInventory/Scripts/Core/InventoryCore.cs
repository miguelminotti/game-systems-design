using MMStdLib.Utils;
using System.Collections.Generic;

namespace REInventory
{
    public interface IInventoryCore
    {
        IReadOnlyList<IRuntimeStorable> Items { get; }
        int MaxCapacity { get; }
        int Width { get; }
        int Height { get; }
        void Initialize(int width, int height);
        bool AddItem(IRuntimeStorable item);
        bool AddItemAtPosition(IRuntimeStorable item, int x, int y);
        bool RemoveItem(IRuntimeStorable item);
    }

    public interface IInventoryChangedEvent
    {
        IInventoryCore RefInventory { get; }
        IRuntimeStorable Item { get; }
        bool IsAdded { get; }
    }

    /// <summary>
    /// Core class for inventory management. Handles item storage, placement, and removal, as well as publishing events when the inventory changes.
    /// </summary>
    public class InventoryCore : IInventoryCore
    {
        public IReadOnlyList<IRuntimeStorable> Items => _items.AsReadOnly();
        public int MaxCapacity => _grid.MaxCapacity;
        public int Width => _grid.Width;
        public int Height => _grid.Height;

        private readonly List<IRuntimeStorable> _items = new List<IRuntimeStorable>();
        private IInventoryGrid _grid;

        public void Initialize(int width, int height)
        {
            _grid = new InventoryGrid(width, height);
        }

        public bool AddItem(IRuntimeStorable item)
        {
            if (_grid.PlaceItemOnAvailableSpace(item))
            {
                item.BindToInventory(this);
                _items.Add(item);
                IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, true);
                GameEventBus.Publish(inventoryChangedEvent);
                return true;
            }
            return false;
        }

        public bool AddItemAtPosition(IRuntimeStorable item, int x, int y)
        {
            if (_grid.PlaceItem(item, x, y))
            {
                item.BindToInventory(this);
                _items.Add(item);
                IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, true);
                GameEventBus.Publish(inventoryChangedEvent);
                return true;
            }
            return false;
        }

        public bool RemoveItem(IRuntimeStorable item)
        {
            if (_items.Remove(item))
            {
                if (_grid.RemoveItem(item))
                {
                    IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, false);
                    GameEventBus.Publish(inventoryChangedEvent);
                    return true;
                }
            }

            return false;
        }

        private class InventoryChangedEvent : IInventoryChangedEvent
        {
            public IInventoryCore RefInventory { get; }
            public IRuntimeStorable Item { get; }
            public bool IsAdded { get; }
            public InventoryChangedEvent(IInventoryCore refInventory, IRuntimeStorable item, bool isAdded)
            {
                RefInventory = refInventory;
                Item = item;
                IsAdded = isAdded;
            }
        }
    }
}