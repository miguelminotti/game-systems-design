using System.Collections.Generic;

namespace REInventory.Core
{
    public interface IInventoryCore
    {
        /// <summary>
        /// Gets a read-only list of all items currently stored in the inventory.
        /// </summary>
        IReadOnlyList<IRuntimeStorable> Items { get; }

        /// <summary>
        /// Gets the maximum number of slots available in the inventory.
        /// </summary>
        int MaxCapacity { get; }

        /// <summary>
        /// Gets the width of the inventory grid.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the inventory grid.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Initializes the inventory using the provided data.
        /// Must be called before performing any operations.
        /// </summary>
        /// <param name="data">Configuration data defining the inventory dimensions.</param>
        void Initialize(IInventoryData data);

        /// <summary>
        /// Attempts to add an item to the first available space in the inventory.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        /// <returns>
        /// A <see cref="IInventoryGrid.PlacementResult"/> indicating the result of the operation.
        /// </returns>
        /// <remarks>
        /// <para><b>On success:</b></para>
        /// <para>- The item is bound to this inventory</para>
        /// <para>- The item is stored internally</para>
        /// <para>- An <see cref="InventoryEvents.IInventoryChangedEvent"/> is published with type <c>Added</c></para>
        /// </remarks>
        IInventoryGrid.PlacementResult TryAddItemOnAvailableSpace(IRuntimeStorable item);

        /// <summary>
        /// Attempts to add an item at a specific grid position.
        /// </summary>
        /// <param name="item">The item to be placed.</param>
        /// <param name="position">The target position in the grid.</param>
        /// <returns>
        /// A <see cref="IInventoryGrid.PlacementResult"/> indicating the result of the operation.
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
        IInventoryGrid.PlacementResult AddItemAtPosition(IRuntimeStorable item, GridPosition position);

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
        bool TryRemoveItem(IRuntimeStorable item);

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
        IInventoryGrid.PlacementResult TryRotateItem(IRuntimeStorable item);

        /// <summary>
        /// Removes all items from the inventory.
        /// </summary>
        /// <remarks>
        /// <para>- Clears all grid slots</para>
        /// <para>- Removes all items from the internal list</para>
        /// <para>- Publishes an <see cref="InventoryEvents.IInventoryClearedEvent"/></para>
        /// </remarks>
        void Clear();
    }
}