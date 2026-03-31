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
        /// <summary>
        /// Gets a read-only list of all items currently stored in the inventory.
        /// </summary>
        public IReadOnlyList<IRuntimeStorable> Items => _items.AsReadOnly();

        /// <summary>
        /// Gets the maximum number of slots available in the inventory.
        /// </summary>
        public int MaxCapacity => _grid.MaxCapacity;

        /// <summary>
        /// Gets the width of the inventory grid.
        /// </summary>
        public int Width => _grid.Width;

        /// <summary>
        /// Gets the height of the inventory grid.
        /// </summary>
        public int Height => _grid.Height;

        private readonly List<IRuntimeStorable> _items = new List<IRuntimeStorable>();
        private IInventoryGrid _grid;

        /// <summary>
        /// Initializes the inventory using the provided data.
        /// Must be called before performing any operations.
        /// </summary>
        /// <param name="data">Configuration data defining the inventory dimensions.</param>
        public void Initialize(IInventoryData data)
        {
            _grid = new InventoryGrid(data.Width, data.Height);
        }

        /// <summary>
        /// Attempts to add an item to the first available space in the inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>
        /// True if the item was successfully placed; otherwise, false if no suitable space was found.
        /// </returns>
        /// <remarks>
        /// <para><b>On success:</b></para>
        /// <para>- The item is bound to this inventory</para>
        /// <para>- The item is stored internally</para>
        /// <para>- An <see cref="InventoryEvents.IInventoryChangedEvent"/> is published with type <c>Added</c></para>
        /// </remarks>
        public bool TryAddItem(IRuntimeStorable item)
        {
            if (_grid.TryPlaceItemOnAvailableSpace(item))
            {
                item.BindToInventory(this);
                _items.Add(item);

                InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Added);

                GameEventBus.Publish(inventoryChangedEvent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to add an item at a specific grid position.
        /// </summary>
        /// <param name="item">The item to be placed.</param>
        /// <param name="position">The target position in the grid.</param>
        /// <returns>
        /// A <see cref="IInventoryGrid.PlaceItemResult"/> indicating the result of the operation.
        /// </returns>
        /// <remarks>
        /// <para><b>On success:</b></para>
        /// <para>- The item is bound to this inventory</para>
        /// <para>- The item is stored internally</para>
        /// <para>- An <see cref="InventoryEvents.IInventoryChangedEvent"/> is published with type <c>Added</c></para>
        /// 
        /// <para><b>On failure:</b></para>
        /// <para>- No changes are applied to the inventory</para>
        /// </remarks>
        public IInventoryGrid.PlaceItemResult AddItemAtPosition(IRuntimeStorable item, GridPosition position)
        {
            IInventoryGrid.PlaceItemResult placeItemResult = _grid.PlaceItem(item, position);

            if (placeItemResult == IInventoryGrid.PlaceItemResult.Succeeded)
            {
                item.BindToInventory(this);
                _items.Add(item);

                InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Added);

                GameEventBus.Publish(inventoryChangedEvent);
            }

            return placeItemResult;
        }

        /// <summary>
        /// Attempts to remove an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>
        /// True if the item was successfully removed; otherwise, false if the item was not found.
        /// </returns>
        /// <remarks>
        /// <para><b>On success:</b></para>
        /// <para>- The item is removed from the grid and internal list</para>
        /// <para>- An <see cref="InventoryEvents.IInventoryChangedEvent"/> is published with type <c>Removed</c></para>
        /// </remarks>
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

        /// <summary>
        /// Attempts to rotate an item within the inventory.
        /// </summary>
        /// <param name="item">The item to rotate.</param>
        /// <returns>
        /// True if the item was successfully rotated; otherwise, false if rotation was not possible.
        /// </returns>
        /// <remarks>
        /// <para><b>Rotation will fail if:</b></para>
        /// <para>- The rotated shape would go out of bounds</para>
        /// <para>- The rotated shape would overlap another item</para>
        /// 
        /// <para><b>On success:</b></para>
        /// <para>- The item orientation is updated in the grid</para>
        /// <para>- An <see cref="InventoryEvents.IInventoryChangedEvent"/> is published with type <c>Rotated</c></para>
        /// </remarks>
        public bool TryRotateItem(IRuntimeStorable item)
        {
            if (_grid.TryRotateItem(item))
            {
                InventoryEvents.IInventoryChangedEvent inventoryChangedEvent =
                    new InventoryChangedEvent(this, item, InventoryEvents.InventoryChangeType.Rotated);

                GameEventBus.Publish(inventoryChangedEvent);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all items from the inventory.
        /// </summary>
        /// <remarks>
        /// <para>- Clears all grid slots</para>
        /// <para>- Removes all items from the internal list</para>
        /// <para>- Publishes an <see cref="InventoryEvents.IInventoryClearedEvent"/></para>
        /// </remarks>
        public void Clear()
        {
            foreach (var item in _items)
            {
                _grid.TryRemoveItem(item);
            }

            _items.Clear();

            GameEventBus.Publish<InventoryEvents.IInventoryClearedEvent, IInventoryCore>(this);
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
    }
}