// TODO: Add support to other item shapes (L, T, etc) - This will likely require a more complex system for tracking occupied slots and checking for valid placements, as well as changes to the IRuntimeStorable interface to define the shape of the item. For now, we can focus on rectangular items and consider shape support as a future enhancement.

using static REInventory.Core.IInventoryGrid;

namespace REInventory.Core
{
    /// <summary>
    /// Represents a grid-based spatial system responsible for validating and applying item placement.
    /// </summary>
    public class InventoryGrid : IInventoryGrid
    {
        /// <inheritdoc/>
        public int MaxCapacity { get; }
        /// <inheritdoc/>
        public int Width { get; }
        /// <inheritdoc/>
        public int Height { get; }

        private readonly InventoryGridSlot[,] _gridSlots;

        /// <summary>
        /// Initialize a new inventory grid with the specified dimensions.
        /// </summary>
        /// <param name="width">The width of the grid.</param>
        /// <param name="height">The height of the grid.</param>
        public InventoryGrid(int width, int height)
        {
            Width = width;
            Height = height;
            MaxCapacity = width * height;

            _gridSlots = new InventoryGridSlot[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _gridSlots[x, y] = new InventoryGridSlot(x, y);
                }
            }
        }

        /// <inheritdoc/>
        public bool TryGetSlot(GridPosition gridPosition, out IInventoryGridSlot gridSlot)
        {
            gridSlot = null;
            if (IsOutOfBounds(gridPosition))
            {
                // Debug message
                return false;
            }
            gridSlot = _gridSlots[gridPosition.X, gridPosition.Y];
            return true;
        }

        /// <inheritdoc/>
        public PlacementResult CheckPlaceItem(IRuntimeStorable item, GridPosition gridPosition)
        {
            GridPosition origin = gridPosition;
            GridPosition[] occupiedPositions;

            int itemWidth = item.Width;
            int itemHeight = item.Height;
            IsPlaceableAtResult isPlacebleAt = IsPlaceableAt(gridPosition, itemWidth, itemHeight);

            switch (isPlacebleAt)
            {
                case IsPlaceableAtResult.Placeable:

                    occupiedPositions = GetOccupiedPositions(item, origin);
                    return PlacementResult.SuccessResult(origin, occupiedPositions);

                case IsPlaceableAtResult.OutOfBounds:

                    return PlacementResult.Failure(PlaceItemResultFailureReason.FailedOutOfBounds);

                case IsPlaceableAtResult.Occupied:

                    return PlacementResult.Failure(PlaceItemResultFailureReason.FailedOccupied);
            }

            return PlacementResult.Failure(PlaceItemResultFailureReason.FailedUnknown);
        }

        /// <inheritdoc/>
        public PlacementResult CheckPlaceItemOnAvailableSpace(IRuntimeStorable item)
        {
            GridPosition origin = default;
            GridPosition[] occupiedPositions;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    IsPlaceableAtResult isPlacebleAt = IsPlaceableAt(position, item.Width, item.Height);

                    if (isPlacebleAt == IsPlaceableAtResult.Placeable)
                    {
                        origin = position;
                        occupiedPositions = GetOccupiedPositions(item, position);
                        return PlacementResult.SuccessResult(origin, occupiedPositions);
                    }
                }
            }

            return PlacementResult.Failure(PlaceItemResultFailureReason.FailedAvailablePositionNotFound);
        }

        /// <inheritdoc/>
        public void ApplyPlacement(IRuntimeStorable item, PlacementResult placementResult)
        {
            FillSlotsForItem(item, placementResult.OccupiedPositions);
        }

        /// <inheritdoc/>
        public bool TryRemoveItem(IRuntimeStorable item)
        {
            bool itemFound = false;
            foreach (GridPosition position in GridUtils.GetRectFromOrigin(item.StartingPosition, item.Width, item.Height))
            {
                if (TryGetSlot(position, out IInventoryGridSlot gridSlot) && gridSlot.StoredItem == item)
                {
                    gridSlot.RemoveItemStored();
                    itemFound = true;
                }
            }
            return itemFound;
        }

        /// <inheritdoc/>
        public IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight)
        {
            foreach (GridPosition checkPosition in GridUtils.GetRectFromOrigin(gridPosition, itemWidth, itemHeight))
            {
                if (checkPosition.X >= Width || checkPosition.Y >= Height)
                {
                    return IsPlaceableAtResult.OutOfBounds;
                }
                else if (TryGetSlot(checkPosition, out IInventoryGridSlot gridSlot) && gridSlot.IsOccupied())
                {
                    return IsPlaceableAtResult.Occupied;
                }
            }

            return IsPlaceableAtResult.Placeable;
        }

        /// <inheritdoc/>
        public PlacementResult CheckRotateItem(IRuntimeStorable item)
        {
            GridPosition origin = item.StartingPosition;
            GridPosition[] occupiedPositions;

            var rotationCheckResult = CanRotateItem(item);

            switch (rotationCheckResult)
            {
                case IsPlaceableAtResult.Placeable:

                    occupiedPositions = GetOccupiedPositions(item.Height, item.Width, item.StartingPosition);
                    return PlacementResult.SuccessResult(origin, occupiedPositions);

                case IsPlaceableAtResult.OutOfBounds:

                    return PlacementResult.Failure(PlaceItemResultFailureReason.FailedOutOfBounds);

                case IsPlaceableAtResult.Occupied:

                    return PlacementResult.Failure(PlaceItemResultFailureReason.FailedOccupied);
            }

            return PlacementResult.Failure(PlaceItemResultFailureReason.FailedUnknown);
        }

        /// <summary>
        /// Determinates if a grid position is out of bounds for the inventory grid dimensions.
        /// </summary>
        /// <param name="gridPosition">The position to be checked.</param>
        /// <returns></returns>
        private bool IsOutOfBounds(GridPosition gridPosition)
        {
            return gridPosition.X < 0 || gridPosition.X >= Width || gridPosition.Y < 0 || gridPosition.Y >= Height;
        }

        /// <summary>
        /// Determines whether the specified item can be rotated within the inventory grid without overlapping other
        /// items or exceeding grid boundaries.
        /// </summary>
        /// <param name="item">The item to evaluate for rotation within the grid.</param>
        /// <returns>An <see cref="IsPlaceableAtResult"/> value indicating whether the item can be rotated: Placeable if rotation is possible,
        /// Occupied if another item blocks the rotation, or OutOfBounds if the rotation would exceed grid limits.</returns>
        private IsPlaceableAtResult CanRotateItem(IRuntimeStorable item)
        {
            foreach (GridPosition position in GridUtils.GetRectFromOrigin(item.StartingPosition, item.Height, item.Width))
            {
                if (TryGetSlot(position, out IInventoryGridSlot gridSlot))
                {
                    if (gridSlot.StoredItem != item && gridSlot.IsOccupied())
                    {
                        return IsPlaceableAtResult.Occupied;
                    }
                }
                else
                {
                    return IsPlaceableAtResult.OutOfBounds;
                }
            }

            return IsPlaceableAtResult.Placeable;
        }

        /// <summary>
        /// Fills the inventory grid slots corresponding to the specified grid positions with the given item.
        /// </summary>
        /// <param name="item">The item to fill the slots.</param>
        /// <param name="gridPositions">The grid positions to filled.</param>
        private void FillSlotsForItem(IRuntimeStorable item, GridPosition[] gridPositions)
        {
            foreach (GridPosition gridPosition in gridPositions)
            {
                if (TryGetSlot(gridPosition, out IInventoryGridSlot gridSlot))
                {
                    gridSlot.StoreItem(item);
                }
            }
        }

        /// <summary>
        /// Calculates the grid positions occupied by the specified item when placed at the given origin.
        /// </summary>
        /// <param name="item">The item for which to determine occupied grid positions. Must provide valid width and height properties.</param>
        /// <param name="origin">The origin position on the grid where the item is placed.</param>
        /// <returns>An array of grid positions occupied by the item at the specified origin. The array will be empty if the item
        /// does not occupy any positions.</returns>
        private GridPosition[] GetOccupiedPositions(IRuntimeStorable item, GridPosition origin)
        {
            return GetOccupiedPositions(item.Width, item.Height, origin);
        }

        /// <summary>
        /// Calculates the grid positions occupied by a rectangular area defined by the specified width and height when placed at the given origin.
        /// </summary>
        /// <param name="width">The specified width of the rectangular area.</param>
        /// <param name="height">The specified height of the rectangular area.</param>
        /// <param name="origin">The origin position on the grid where the rectangular area starts.</param>
        /// <returns></returns>
        private GridPosition[] GetOccupiedPositions(int width, int height, GridPosition origin)
        {
            var positions = new GridPosition[width * height];

            int count = 0;
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    positions[count] = origin.Move(i, j);
                    count++;
                }
            }

            return positions;
        }
    }
}