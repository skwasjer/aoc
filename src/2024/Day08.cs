using System.Numerics;
using Aoc;
using Aoc.NUnit;

namespace _2024;

public sealed class Day08 : Puzzle<char[,]>
{
    private const char AntiNode = '#';

    protected override char[,] GetInput(Stream stream)
    {
        return stream.Read2DArray();
    }

    [Solution(14)]
    public static long Part1(char[,] map)
    {
        return Solve(map,
            (a, b) =>
            {
                Vector2 distance = a.Position - b.Position;
                TryAddAntiNode(map, a.Position + distance);
                TryAddAntiNode(map, b.Position - distance);
            });
    }

    [Solution(34)]
    public static long Part2(char[,] map)
    {
        return Solve(map,
            (a, b) =>
            {
                Vector2 distance = a.Position - b.Position;
                Vector2 ap = a.Position;
                Vector2 bp = b.Position;
                while (TryAddAntiNode(map, ap)) { ap += distance; }

                while (TryAddAntiNode(map, bp)) { bp += distance; }
            });
    }

    private static int Solve(char[,] map, Action<Antenna, Antenna> addAntiNodes)
    {
        foreach ((Antenna a, Antenna b) in map.FindPositions(char.IsLetterOrDigit)
                     .Select(pos => new Antenna(new Vector2(pos.x, pos.y), map[pos.x, pos.y]))
                     .GroupBy(g => g.Id, (id, lst) => new AntennaGroup(id, lst))
                     .SelectMany(a => a.GetPairs()))
        {
            addAntiNodes(a, b);
        }

        return map.FindPositions(AntiNode).Count();
    }

    private static bool TryAddAntiNode(char[,] map, Vector2 pos)
    {
        if (pos.X < 0 || pos.X >= map.GetLength(0)
         || pos.Y < 0 || pos.Y >= map.GetLength(1))
        {
            // Out of bounds.
            return false;
        }

        map[(int)pos.X, (int)pos.Y] = AntiNode;
        return true;
    }

    private class AntennaGroup(char id, IEnumerable<Antenna> antennae) : List<Antenna>(antennae)
    {
        public char Id { get => id; }

        public IEnumerable<(Antenna, Antenna)> GetPairs()
        {
            return from a in this
                   from b in this
                   where a != b
                   select (a, b);
        }
    }

    private readonly record struct Antenna(Vector2 Position, char Id);
}
