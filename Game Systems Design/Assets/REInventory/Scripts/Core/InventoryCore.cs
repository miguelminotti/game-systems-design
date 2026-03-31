using MMStdLib.Utils;
using System.Collections.Generic;

namespace REInventory.Core
{
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

        public void Initialize(IInventoryData data)
        {
            _grid = new InventoryGrid(data.Width, data.Height);
        }

        public bool TryAddItem(IRuntimeStorable item)
        {
            if (_grid.TryPlaceItemOnAvailableSpace(item))
            {
                item.BindToInventory(this);
                _items.Add(item);
                IInventoryCore.IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, true);
                GameEventBus.Publish(inventoryChangedEvent);
                return true;
            }
            return false;
        }

        public IInventoryGrid.PlaceItemResult AddItemAtPosition(IRuntimeStorable item, GridPosition position)
        {
            IInventoryGrid.PlaceItemResult placeItemResult = _grid.PlaceItem(item, position);
            if (placeItemResult == IInventoryGrid.PlaceItemResult.Succeeded)
            {
                item.BindToInventory(this);
                _items.Add(item);
                IInventoryCore.IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, true);
                GameEventBus.Publish(inventoryChangedEvent);
                return placeItemResult;
            }
            return placeItemResult;
        }

        public bool TryRemoveItem(IRuntimeStorable item)
        {
            if (_grid.TryRemoveItem(item))
            {
                if (_items.Remove(item))
                {
                    IInventoryCore.IInventoryChangedEvent inventoryChangedEvent = new InventoryChangedEvent(this, item, true);
                    GameEventBus.Publish(inventoryChangedEvent);
                    return true;
                }
            }

            return false;
        }

        public bool TryRotateItem(IRuntimeStorable item)
        {
            return _grid.TryRotateItem(item);
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                _grid.TryRemoveItem(item);
            }

            _items.Clear();

            GameEventBus.Publish<IInventoryCore.IInventoryClearedEvent, IInventoryCore>(this);
        }

        private class InventoryChangedEvent : IInventoryCore.IInventoryChangedEvent
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