namespace REInventory.Core
{
    public interface IInventoryGrid
    {
        int MaxCapacity { get; }
        int Width { get; }
        int Height { get; }

        bool TryGetSlot(GridPosition gridPosition, out IInventoryGridSlot gridSlot);
        enum PlaceItemResultFailureReason
        {
            FailedOutOfBounds,
            FailedOccupied,
            FailedAvailablePositionNotFound,
            FailedUnknown
        }
        public readonly struct PlacementResult
        {
            public bool Success { get; }
            public GridPosition Origin { get; }
            public GridPosition[] OccupiedPositions { get; }
            public PlaceItemResultFailureReason FailureReason { get; }

            public PlacementResult(bool success, GridPosition origin, GridPosition[] occupiedPositions, PlaceItemResultFailureReason failureReason)
            {
                Success = success;
                Origin = origin;
                OccupiedPositions = occupiedPositions;
                FailureReason = failureReason;
            }

            public static PlacementResult Failure(PlaceItemResultFailureReason failureReason)
            {
                return new PlacementResult(false, default, null, failureReason);
            }

            public static PlacementResult SuccessResult(GridPosition origin, GridPosition[] occupiedPositions)
            {
                return new PlacementResult(true, origin, occupiedPositions, default);
            }
        }
        PlacementResult CheckPlaceItem(IRuntimeStorable item, GridPosition gridPosition);
        PlacementResult CheckPlaceItemOnAvailableSpace(IRuntimeStorable item);
        PlacementResult CheckRotateItem(IRuntimeStorable item);
        void ApplyPlacement(IRuntimeStorable item, PlacementResult placementResult);
        bool TryRemoveItem(IRuntimeStorable item);
        enum IsPlaceableAtResult
        {
            Placeable,
            OutOfBounds,
            Occupied
        }
        IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight);
    }
}