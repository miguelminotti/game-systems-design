namespace REInventory.Core
{
    /// <summary>
    /// Represents a grid-based spatial system responsible for validating and applying item placement.
    /// </summary>
    /// <remarks>
    /// <para>This interface defines a two-phase workflow for grid operations:</para>
    /// <para>1. <b>Check phase</b> – validates operations and returns a <see cref="PlacementResult"/> without mutating state</para>
    /// <para>2. <b>Apply phase</b> – applies a previously validated result to mutate the grid</para>
    ///
    /// <para>This separation ensures deterministic behavior, enabling safe simulations, previews, and undo systems.</para>
    /// </remarks>
    public interface IInventoryGrid
    {
        /// <summary>
        /// Gets the maximum number of slots available in the grid.
        /// </summary>
        int MaxCapacity { get; }

        /// <summary>
        /// Gets the width of the grid.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Attempts to retrieve a grid slot at the specified position.
        /// </summary>
        /// <param name="gridPosition">The position to query.</param>
        /// <param name="gridSlot">The resulting slot if found.</param>
        /// <returns>
        /// True if the position is valid and a slot exists; otherwise, false if out of bounds.
        /// </returns>
        bool TryGetSlot(GridPosition gridPosition, out IInventoryGridSlot gridSlot);

        /// <summary>
        /// Defines failure reasons for placement-related operations.
        /// </summary>
        public enum PlaceItemResultFailureReason
        {
            /// <summary>Placement failed because the item exceeds grid boundaries.</summary>
            FailedOutOfBounds,

            /// <summary>Placement failed because one or more slots are already occupied.</summary>
            FailedOccupied,

            /// <summary>Placement failed because no suitable position was found.</summary>
            FailedAvailablePositionNotFound,

            /// <summary>Placement failed due to an unspecified or unexpected reason.</summary>
            FailedUnknown
        }

        /// <summary>
        /// Represents the result of a placement-related operation.
        /// </summary>
        /// <remarks>
        /// <para>This struct is used for both validation and execution phases.</para>
        /// <para>When <see cref="Success"/> is true, <see cref="Origin"/> and <see cref="OccupiedPositions"/> are valid.</para>
        /// <para>When false, <see cref="FailureReason"/> describes the cause of failure.</para>
        /// </remarks>
        public readonly struct PlacementResult
        {
            /// <summary>
            /// Gets whether the operation was successful.
            /// </summary>
            public bool Success { get; }

            /// <summary>
            /// Gets the origin position used for the operation.
            /// </summary>
            public GridPosition Origin { get; }

            /// <summary>
            /// Gets the grid positions that would be occupied by the item.
            /// </summary>
            public GridPosition[] OccupiedPositions { get; }

            /// <summary>
            /// Gets the reason for failure when <see cref="Success"/> is false.
            /// </summary>
            public PlaceItemResultFailureReason FailureReason { get; }

            public PlacementResult(bool success, GridPosition origin, GridPosition[] occupiedPositions, PlaceItemResultFailureReason failureReason)
            {
                Success = success;
                Origin = origin;
                OccupiedPositions = occupiedPositions;
                FailureReason = failureReason;
            }

            /// <summary>
            /// Creates a failed result with the specified reason.
            /// </summary>
            public static PlacementResult Failure(PlaceItemResultFailureReason failureReason)
            {
                return new PlacementResult(false, default, null, failureReason);
            }

            /// <summary>
            /// Creates a successful result with origin and occupied positions.
            /// </summary>
            public static PlacementResult SuccessResult(GridPosition origin, GridPosition[] occupiedPositions)
            {
                return new PlacementResult(true, origin, occupiedPositions, default);
            }
        }

        /// <summary>
        /// Validates whether an item can be placed at a specific position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="gridPosition">The target position.</param>
        /// <returns>
        /// A <see cref="PlacementResult"/> describing the outcome of the validation.
        /// </returns>
        /// <remarks>
        /// <para>This method does not modify the grid.</para>
        /// <para><b>On success:</b> returns the positions the item would occupy.</para>
        /// <para><b>On failure:</b> returns the appropriate <see cref="PlaceItemResultFailureReason"/>.</para>
        /// </remarks>
        PlacementResult CheckPlaceItem(IRuntimeStorable item, GridPosition gridPosition);

        /// <summary>
        /// Validates placement of an item in the first available space in the grid.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <returns>
        /// A <see cref="PlacementResult"/> describing the outcome of the search.
        /// </returns>
        /// <remarks>
        /// <para>This method scans the grid from top-left to bottom-right.</para>
        /// <para>This method does not modify the grid.</para>
        /// </remarks>
        PlacementResult CheckPlaceItemOnAvailableSpace(IRuntimeStorable item);

        /// <summary>
        /// Validates whether an item can be rotated at its current position.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <returns>
        /// A <see cref="PlacementResult"/> describing the outcome of the rotation check.
        /// </returns>
        /// <remarks>
        /// <para>Rotation swaps the item's width and height.</para>
        /// <para><b>Fails if:</b></para>
        /// <para>- The rotated shape exceeds grid bounds</para>
        /// <para>- The rotated shape overlaps another item</para>
        /// <para>This method does not modify the grid.</para>
        /// </remarks>
        PlacementResult CheckRotateItem(IRuntimeStorable item);

        /// <summary>
        /// Applies a previously validated placement to the grid.
        /// </summary>
        /// <param name="item">The item to place.</param>
        /// <param name="placementResult">The result obtained from a successful check operation.</param>
        /// <remarks>
        /// <para>This method mutates the grid state.</para>
        /// <para>It assumes the provided <see cref="PlacementResult"/> is valid and successful.</para>
        /// </remarks>
        void ApplyPlacement(IRuntimeStorable item, PlacementResult placementResult);

        /// <summary>
        /// Attempts to remove an item from the grid.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>
        /// True if the item was found and removed; otherwise, false.
        /// </returns>
        bool TryRemoveItem(IRuntimeStorable item);

        /// <summary>
        /// Represents the result of a placement validation at a specific position.
        /// </summary>
        public enum IsPlaceableAtResult
        {
            /// <summary>The position is valid and unoccupied.</summary>
            Placeable,

            /// <summary>The position exceeds grid boundaries.</summary>
            OutOfBounds,

            /// <summary>The position is already occupied.</summary>
            Occupied
        }

        /// <summary>
        /// Checks whether a rectangular area is placeable at a given position.
        /// </summary>
        /// <param name="gridPosition">The origin position.</param>
        /// <param name="itemWidth">The width of the item.</param>
        /// <param name="itemHeight">The height of the item.</param>
        /// <returns>
        /// A <see cref="IsPlaceableAtResult"/> indicating whether the area is valid.
        /// </returns>
        /// <remarks>
        /// <para>This is a low-level validation method used internally by placement checks.</para>
        /// <para>It does not modify the grid.</para>
        /// </remarks>
        IsPlaceableAtResult IsPlaceableAt(GridPosition gridPosition, int itemWidth, int itemHeight);
    }
}