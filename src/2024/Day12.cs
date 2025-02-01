using System.Numerics;
using Aoc;
using Aoc.NUnit;
using static _2024.Day12;
using static _2024.Primitives.Compass.CardinalDirection;
using static _2024.Primitives.Compass.OrdinalDirection;

namespace _2024;

public sealed class Day12 : Puzzle<List<Region>>
{
    protected override List<Region> GetInput(Stream stream)
    {
        Plot[,] plots = stream.Read2DArray((ch, x, y) => new Plot(ch, new Vector2(x, y)));
        return Region.ScanAll(plots).ToList();
    }

    [Solution(1930)]
    public static int Part1(List<Region> regions)
    {
        return regions.Sum(r => r.Plots.Count * r.GetPerimeter());
    }

    [Solution(1206)]
    public static int Part2(List<Region> regions)
    {
        return regions.Sum(r => r.Plots.Count * r.GetSides());
    }

    public sealed record Plot(char Type, Vector2 Location)
    {
        public Region? Region { get; internal set; }

        public HashSet<Plot> Neighbours { get; } = [];

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, Location);
        }

        internal int CornerCount()
        {
            if (Region is null)
            {
                throw new InvalidOperationException("Region not initialized.");
            }

            // Outer corners
            return Convert.ToInt32(IsOuterCorner(West, North))
              + Convert.ToInt32(IsOuterCorner(North, East))
              + Convert.ToInt32(IsOuterCorner(East, South))
              + Convert.ToInt32(IsOuterCorner(South, West))
                // Inner corners
              + Convert.ToInt32(IsInnerCorner(West, North))
              + Convert.ToInt32(IsInnerCorner(North, East))
              + Convert.ToInt32(IsInnerCorner(East, South))
              + Convert.ToInt32(IsInnerCorner(South, West));

            bool IsOuterCorner(Vector2 a, Vector2 b)
            {
                return ((Region.Source.TryGetAt(Location + a, out Plot? pa) && pa.Type != Type) || pa is null)
                 && ((Region.Source.TryGetAt(Location + b, out Plot? pb) && pb.Type != Type) || pb is null);
            }

            bool IsInnerCorner(Vector2 a, Vector2 b)
            {
                return Region.Source.TryGetAt(Location + a, out Plot? pa) && pa.Type == Type
                 && ((Region.Source.TryGetAt(Location + a + b, out Plot? pab) && pab.Type != Type) || pab is null)
                 && Region.Source.TryGetAt(Location + b, out Plot? pb) && pb.Type == Type;
            }
        }
    }

    public sealed record Region(char Type, HashSet<Plot> Plots, Plot[,] Source)
    {
        public int GetPerimeter()
        {
            return Plots.Sum(p => 4 - p.Neighbours.Count);
        }

        public int GetSides()
        {
            return Plots.Sum(p => p.CornerCount());
        }

        internal static IEnumerable<Region> ScanAll(Plot[,] plots)
        {
            Vector2[] directions = [North, East, South, West];

            return plots.GetAll()
                .GroupBy(plot => plot.value.Type)
                .SelectMany(g =>
                {
                    var regions = new List<Region>();

                    // Link neighbouring plots.
                    foreach ((_, Plot plot) in g)
                    {
                        foreach (Vector2 dir in directions)
                        {
                            if (!plots.TryGetAt(plot.Location + dir, out Plot? neighbour)
                             || neighbour.Type != plot.Type)
                            {
                                continue;
                            }

                            neighbour.Neighbours.Add(plot);
                            plot.Neighbours.Add(neighbour);
                        }

                        Region[] neighbourRegions = plot.Neighbours
                            .Select(n => n.Region)
                            .Where(r => r is not null)
                            .OfType<Region>()
                            .Distinct()
                            .ToArray();

                        Region? region = neighbourRegions.FirstOrDefault();
                        // Merge regions?
                        if (region is not null && neighbourRegions.Length > 1)
                        {
                            for (int i = 1; i < neighbourRegions.Length; i++)
                            {
                                Region r = neighbourRegions[i];
                                regions.Remove(r);
                                foreach (Plot p in r.Plots)
                                {
                                    region.Plots.Add(p);
                                    p.Region = region;
                                }
                            }
                        }

                        if (region is null)
                        {
                            region = new Region(plot.Type, [], plots);
                            regions.Add(region);
                        }

                        region.Plots.Add(plot);
                        plot.Region = region;
                    }

                    return regions;
                });
        }
    }
}
