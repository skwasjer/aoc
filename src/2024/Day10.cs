using System.Numerics;
using Aoc;
using Aoc.NUnit;
using static _2024.Day10;
using static _2024.Primitives.Compass.CardinalDirection;

namespace _2024;

public sealed class Day10 : Puzzle<HeightMap>
{
    protected override HeightMap GetInput(Stream stream)
    {
        Location[,] map = stream
            .Read2DArray((ch, x, y) =>
            {
                int height = ch - '0';
                return new Location(new Vector2(x, y), height);
            });

        return new HeightMap(map);
    }

    [Solution(36)]
    public static int Part1(HeightMap map)
    {
        return map
            .CountHeadTrails(true)
            .Sum(x => x.RouteCount);
    }

    [Solution(81)]
    public static int Part2(HeightMap map)
    {
        return map
            .CountHeadTrails(false)
            .Sum(x => x.RouteCount);
    }

    public sealed class HeightMap
    {
        private const int MinHeight = 0;
        internal const int MaxHeight = 9;

        private readonly Location[,] _heightMap;

        public HeightMap(Location[,] heightMap)
        {
            _heightMap = heightMap;

            Vector2[] directions = [North, West, South, East];
            foreach ((_, Location value) in heightMap.GetAll())
            {
                foreach (Vector2 dir in directions)
                {
                    // Link neighbours.
                    Vector2 neighbourPos = value.Position + dir;
                    if (!heightMap.TryGetAt(neighbourPos, out Location neighbour)
                     || neighbour.Height != value.Height + 1)
                    {
                        continue;
                    }

                    value.Next.Add(neighbour);
                }
            }
        }

        public IEnumerable<(Location Start, int RouteCount)> CountHeadTrails(bool anyRoute)
        {
            return _heightMap.GetAll()
                .Where(o => o.value.Height == MinHeight)
                .Select(o => (o.value, AnyRoute(o.value.FindPeaks()).Count()));

            IEnumerable<Location> AnyRoute(IEnumerable<Location> e) => anyRoute ? e.Distinct() : e;
        }
    }

    public readonly record struct Location(Vector2 Position, int Height)
    {
        public HashSet<Location> Next { get; } = new();

        public IEnumerable<Location> FindPeaks()
        {
            return FindPeaks(this, HeightMap.MaxHeight);
        }

        private static IEnumerable<Location> FindPeaks(Location step, int height)
        {
            bool isPeak = step.Next.Count == 0;
            if (isPeak)
            {
                return height == step.Height ? [step] : [];
            }

            return step.Next.SelectMany(nextStep => FindPeaks(nextStep, height));
        }
    }
}
