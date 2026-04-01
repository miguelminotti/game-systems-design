// TODO: Fix rotation operation

using System.Collections.Generic;

namespace REInventory.Core
{
    public static class GridUtils
    {
        public static IEnumerable<GridPosition> GetRectFromOrigin(GridPosition origin, int width, int height)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    yield return origin.Move(x, y);
                }
            }
        }

        public static IEnumerable<GridPosition> GetCircleGridSpace(GridPosition center, int radius)
        {
            int rSquared = radius * radius;

            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    if (x * x + y * y <= rSquared)
                    {
                        yield return new GridPosition(center.X + x, center.Y + y);
                    }
                }
            }
        }

        public static IEnumerable<GridPosition> GetDiamondGridSpace(GridPosition center, int radius)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int maxY = radius - System.Math.Abs(x);

                for (int y = -maxY; y <= maxY; y++)
                {
                    yield return new GridPosition(center.X + x, center.Y + y);
                }
            }
        }
    }
}