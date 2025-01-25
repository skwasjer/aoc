using System.Numerics;

namespace _2024.Primitives;

internal static class Compass
{
    internal static class CardinalDirection
    {
        public static readonly Vector2 North = new(0, -1);
        public static readonly Vector2 South = new(0, 1);
        public static readonly Vector2 East = new(1, 0);
        public static readonly Vector2 West = new(-1, 0);
    }

    internal static class OrdinalDirection
    {
        public static readonly Vector2 NorthEast = new(1, -1);
        public static readonly Vector2 NorthWest = new(-1, -1);
        public static readonly Vector2 SouthEast = new(1, 1);
        public static readonly Vector2 SouthWest = new(-1, 1);
    }
}
