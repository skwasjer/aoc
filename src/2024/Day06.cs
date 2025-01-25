using System.Numerics;
using Aoc;
using Aoc.NUnit;
using static _2024.Primitives.Compass.CardinalDirection;

namespace _2024;

public sealed class Day06 : Puzzle<char[,]>
{
    private const char Obstacle = '#';
    private const char Guard = '^';

    protected override char[,] GetInput(Stream stream)
    {
        return stream.Read2DArray();
    }

    [Solution(41)]
    public static int Part1(char[,] input)
    {
        Vector2 guardPos = input.FindPositions(Guard)
            .Select(p => new Vector2(p.x, p.y))
            .Single();
        return WalkPath(input, guardPos, North)
            .Select(p => p.Position)
            .ToHashSet()
            .Count;
    }

    [Solution(6)]
    public static int Part2(char[,] input)
    {
        Vector2 guardPos = input.FindPositions(Guard)
            .Select(p => new Vector2(p.x, p.y))
            .Single();

        var path = WalkPath(input, guardPos, North)
            .Select(p => p.Position)
            .ToHashSet();

        int loopCounter = 0;
        var loopDetection = new Dictionary<(Vector2 Position, Vector2 Orientation, bool IsPathBlocked), int>();
        // For each step in the path, replace it with an obstacle and walk the path again to test for loop.
        // This is a brute force solution, since we re-walk the path each time.
        // The only optimization is that we do not test positions the guard never walks.
        foreach (Vector2 step in path)
        {
            // Replace with obstacle.
            char cache = input[(int)step.X, (int)step.Y];
            input[(int)step.X, (int)step.Y] = Obstacle;

            try
            {
                loopDetection.Clear();
                // If path intersects itself, we are in a loop.
                if (WalkPath(input, guardPos, North)
                    .Any(p => !loopDetection.TryAdd(p, 1)))
                {
                    loopCounter++;
                }
            }
            finally
            {
                // Restore to original.
                input[(int)step.X, (int)step.Y] = cache;
            }
        }

        return loopCounter;
    }

    private static IEnumerable<(Vector2 Position, Vector2 Orientation, bool IsPathBlocked)> WalkPath
    (
        char[,] input,
        Vector2 pos,
        Vector2 orientation
    )
    {
        var rotCw90 = Matrix3x2.CreateRotation(float.DegreesToRadians(90));

        while (true)
        {
            bool isEndOfPath = !input.TryGetAt(pos + orientation, out char? ch);
            bool isPathBlocked = !isEndOfPath && ch == Obstacle;

            yield return (pos, orientation, isPathBlocked);

            if (isEndOfPath)
            {
                break;
            }

            // Take next step, turning right if needed.
            if (isPathBlocked)
            {
                orientation = Vector2.Transform(orientation, rotCw90);
                continue;
            }

            pos += orientation;
        }
    }
}
