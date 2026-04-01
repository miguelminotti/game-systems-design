using MMStdLib.Utils;
using System.Collections.Generic;

namespace REInventory.Core
{
    /// <summary>
    /// Provides the main entry point for interacting with the inventory system.
    /// Handles item storage, placement, removal, rotation, and lifecycle management.
    /// </summary>
    /// <remarks>
    /// <para>This class acts as a facade over the internal grid system, exposing high-level operations
    /// for adding, removing, and manipulating items.</para>
    /// 
    /// <para>All successful operations will publish events through the <see cref="GameEventBus"/>.</para>
    /// </remarks>
    public class InventoryCore : IInventoryCore
    {
        /// <inheritdoc/>
        public IReadOnlyList<IRuntimeStorable> Items => _items.AsReadOnly();

        /// <inheritdoc/>
        public int MaxCapacity => _grid.MaxCapacity;

        /// <inheritdoc/>
        public int Width => _grid.Width;

        /// <inheritdoc/>
        public int Height => _grid.Height;

        private readonly List<IRuntimeStorable> _items = new List<IRuntimeStorable>();
        private IInventoryGrid _grid;

        /// <inheritdoc/>
        public void Initialize(IInventoryData data)
        {
            _grid = new InventoryGrid(data.Width, data.Height);
        }

        /// <inheritdoc/>
        public IInventoryGrid.PlacementResult TryAddItemOnAvailableSpace(IRuntimeStorable item)
        {
            var result = _grid.CheckPlaceItemOnAvailableSpace(item);

            if (result.Success)
            {
                item.SetPosition(result.Origin);
                item.BindToInventory(this);
                _grid.ApplyPlacement(item, result);
                _items.Add(item);

                InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Added);

                GameEventBus.Publish(inventoryChangedEvent);
            }

            return result;
        }

        /// <inheritdoc/>
        public IInventoryGrid.PlacementResult AddItemAtPosition(IRuntimeStorable item, GridPosition position)
        {
            var result = _grid.CheckPlaceItem(item, position);

            if (result.Success)
            {
                item.SetPosition(result.Origin);
                item.BindToInventory(this);
                _grid.ApplyPlacement(item, result);
                _items.Add(item);

                InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Added);

                GameEventBus.Publish(inventoryChangedEvent);
            }

            return result;
        }

        /// <inheritdoc/>
        public bool TryRemoveItem(IRuntimeStorable item)
        {
            if (_grid.TryRemoveItem(item))
            {
                if (_items.Remove(item))
                {
                    InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                        new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Removed);

                    GameEventBus.Publish(inventoryChangedEvent);
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public IInventoryGrid.PlacementResult TryRotateItem(IRuntimeStorable item)
        {
            var result = _grid.CheckRotateItem(item);

            if (result.Success)
            {
                if (_grid.TryRemoveItem(item))
                {
                    _grid.ApplyPlacement(item, result);
                    InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Rotated);

                    GameEventBus.Publish(inventoryChangedEvent);
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            foreach (var item in _items)
            {
                _grid.TryRemoveItem(item);
            }

            _items.Clear();

            InventoryEvents.IInventoryClearedEvent inventoryClearedEvent = new InventoryClearedEvent(this);
            GameEventBus.Publish(inventoryClearedEvent);
        }

        /// <summary>
        /// Internal implementation of the inventory changed event.
        /// </summary>
        private class InventoryChangedEvent : InventoryEvents.IInventoryChangedEvent
        {
            /// <inheritdoc/>
            public IInventoryCore RefInventory { get; }

            /// <inheritdoc/>
            public IRuntimeStorable Item { get; }

            /// <inheritdoc/>
            public InventoryEvents.InventoryChangeType ChangeType { get; }

            public InventoryChangedEvent(
                IInventoryCore refInventory,
                IRuntimeStorable item,
                InventoryEvents.InventoryChangeType changeType)
            {
                RefInventory = refInventory;
                Item = item;
                ChangeType = changeType;
            }
        }

        /// <summary>
        /// Internal implementation of the inventory cleared event.
        /// </summary>
        private class InventoryClearedEvent : InventoryEvents.IInventoryClearedEvent
        {
            /// <inheritdoc/>
            public IInventoryCore RefInventory { get; }
            public InventoryClearedEvent(IInventoryCore refInventory)
            {
                RefInventory = refInventory;
            }
        }
    }
}