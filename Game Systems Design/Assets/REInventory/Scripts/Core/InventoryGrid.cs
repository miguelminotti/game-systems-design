using static REInventory.Core.IInventoryGrid;

namespace REInventory.Core
{
    public class InventoryGrid : IInventoryGrid
    {
        public int MaxCapacity { get; }
        public int Width { get; }
        public int Height { get; }

        private readonly InventoryGridSlot[,] _gridSlots;

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

            return PlacementResult.Failure(PlaceItemResultFailureReason.FailedAvailablePositionNotFound); // No available space found for the item
        }

        public void ApplyPlacement(IRuntimeStorable item, PlacementResult placementResult)
        {
            FillSlotsForItem(item, placementResult.OccupiedPositions);
        }

        public bool TryRemoveItem(IRuntimeStorable item)
        {
            bool itemFound = false;

            for (int x = item.StartingPosition.X; x <= item.EndingPosition.X; x++)
            {
                for (int y = item.StartingPosition.Y; y <= item.EndingPosition.Y; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    if (TryGetSlot(position, out IInventoryGridSlot gridSlot) && gridSlot.StoredItem == item)
                    {
                        gridSlot.RemoveItemStored();
                        itemFound = true;
                    }
                }
            }

            return itemFound;
        }

        public IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight)
        {
            for (int i = 0; i < itemWidth; i++)
            {
                for (int j = 0; j < itemHeight; j++)
                {
                    GridPosition checkPosition = gridPosition.Move(i, j);
                    if (checkPosition.X >= Width || checkPosition.Y >= Height)
                    {
                        return IsPlaceableAtResult.OutOfBounds;
                    } else if (TryGetSlot(checkPosition, out IInventoryGridSlot gridSlot) && gridSlot.IsOccupied())
                    {
                        return IsPlaceableAtResult.Occupied;
                    }
                }
            }

            return IsPlaceableAtResult.Placeable; // Placeable if all slots are within bounds and unoccupied
        }

        public bool IsOutOfBounds(GridPosition gridPosition)
        {
            return gridPosition.X < 0 || gridPosition.X >= Width || gridPosition.Y < 0 || gridPosition.Y >= Height;
        }

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

        private IsPlaceableAtResult CanRotateItem(IRuntimeStorable item)
        {
            for (int x = item.StartingPosition.X; x <= item.StartingPosition.X + item.Height - 1; x++)
            {
                for (int y = item.StartingPosition.Y; y <= item.StartingPosition.Y + item.Width - 1; y++)
                {
                    GridPosition position = new GridPosition(x, y);
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
            }
            return IsPlaceableAtResult.Placeable;
        }

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

        private GridPosition[] GetOccupiedPositions(IRuntimeStorable item, GridPosition origin)
        {
            return GetOccupiedPositions(item.Width, item.Height, origin);
        }

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