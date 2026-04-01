// TODO: Add a full support of result structs on each void
// TODO: Separate checking and applying operations

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

        public IInventoryGrid.PlaceItemResult PlaceItem(IRuntimeStorable item, GridPosition gridPosition)
        {
            int itemWidth = item.Width;
            int itemHeight = item.Height;
            IInventoryGrid.IsPlaceableAtResult isPlacebleAt = IsPlaceableAt(gridPosition, itemWidth, itemHeight);
            switch (isPlacebleAt)
            {
                case IInventoryGrid.IsPlaceableAtResult.Placeble:
                    FillSlotsForItem(item, gridPosition);
                    return IInventoryGrid.PlaceItemResult.Succeeded;
                case IInventoryGrid.IsPlaceableAtResult.OutOfBounds:
                    return IInventoryGrid.PlaceItemResult.FailedOutOfBounds;
                case IInventoryGrid.IsPlaceableAtResult.Occupied:
                    return IInventoryGrid.PlaceItemResult.FailedOccupied;
            }
            return IInventoryGrid.PlaceItemResult.FailedOutOfBounds;
        }

        public bool TryPlaceItemOnAvailableSpace(IRuntimeStorable item, out GridPosition placedPosition)
        {
            placedPosition = default;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    GridPosition position = new GridPosition(x, y);
                    IInventoryGrid.IsPlaceableAtResult isPlacebleAt = IsPlaceableAt(position, item.Width, item.Height);

                    if (isPlacebleAt == IInventoryGrid.IsPlaceableAtResult.Placeble)
                    {
                        placedPosition = position;
                        FillSlotsForItem(item, position);
                        return true;
                    }
                }
            }

            return false; // No available space found for the item
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

        public IInventoryGrid.IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight)
        {
            for (int i = 0; i < itemWidth; i++)
            {
                for (int j = 0; j < itemHeight; j++)
                {
                    GridPosition checkPosition = gridPosition.Move(i, j);
                    if (checkPosition.X >= Width || checkPosition.Y >= Height)
                    {
                        return IInventoryGrid.IsPlaceableAtResult.OutOfBounds;
                    } else if (TryGetSlot(checkPosition, out IInventoryGridSlot gridSlot) && gridSlot.IsOccupied())
                    {
                        return IInventoryGrid.IsPlaceableAtResult.Occupied;
                    }
                }
            }

            return IInventoryGrid.IsPlaceableAtResult.Placeble; // Placeable if all slots are within bounds and unoccupied
        }

        public bool IsOutOfBounds(GridPosition gridPosition)
        {
            return gridPosition.X < 0 || gridPosition.X >= Width || gridPosition.Y < 0 || gridPosition.Y >= Height;
        }

        public bool TryRotateItem(IRuntimeStorable item)
        {
            if (CanRotateItem(item))
            {
                if (TryRemoveItem(item))
                {
                    FillSlotsForItem(item, item.Height, item.Width, item.StartingPosition);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private bool CanRotateItem(IRuntimeStorable item)
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
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void FillSlotsForItem(IRuntimeStorable item, GridPosition gridPosition)
        {
            FillSlotsForItem(item, item.Width, item.Height, gridPosition);
        }

        private void FillSlotsForItem(IRuntimeStorable item, int width, int height, GridPosition gridPosition)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (TryGetSlot(gridPosition.Move(i, j), out IInventoryGridSlot gridSlot))
                    {
                        gridSlot.StoreItem(item);
                    }
                }
            }
        }
    }
}