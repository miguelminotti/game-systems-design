namespace REInventory.Core
{
    public interface IInventoryGrid
    {
        int MaxCapacity { get; }
        int Width { get; }
        int Height { get; }

        bool TryGetSlot(GridPosition gridPosition, out IInventoryGridSlot gridSlot);
        enum PlaceItemResult
        {
            Succeeded,
            FailedOutOfBounds,
            FailedOccupied
        }
        PlaceItemResult PlaceItem(IRuntimeStorable item, GridPosition gridPosition);
        bool TryPlaceItemOnAvailableSpace(IRuntimeStorable item, out GridPosition placedPosition);
        bool TryRemoveItem(IRuntimeStorable item);
        enum IsPlaceableAtResult
        {
            Placeble,
            OutOfBounds,
            Occupied
        }
        IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight);
        bool TryRotateItem(IRuntimeStorable item);
    }
}