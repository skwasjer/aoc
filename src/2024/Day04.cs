using System.Numerics;
using Aoc;
using Aoc.NUnit;
using static _2024.Primitives.Compass.CardinalDirection;
using static _2024.Primitives.Compass.OrdinalDirection;

namespace _2024;

public sealed class Day04 : Puzzle<char[,]>
{
    protected override char[,] GetInput(Stream stream)
    {
        return stream.Read2DArray();
    }

    [Solution(18)]
    public static int Part1(char[,] input)
    {
        Vector2[] directions = [North, South, East, West, NorthEast, NorthWest, SouthEast, SouthWest];
        return input.FindPositions('X')
            .Sum(pos => directions
                .Sum(direction =>
                {
                    bool isMatch = IsWordMatch(input, "XMAS", pos.x, pos.y, direction);
                    return Convert.ToInt32(isMatch);
                })
            );
    }

    [Solution(9)]
    public static int Part2(char[,] input)
    {
        Vector2[] directions = [NorthEast, NorthWest, SouthEast, SouthWest];
        return input.FindPositions('A')
            .Count(pos =>
            {
                var center = new Vector2(pos.x, pos.y);
                return directions.Sum(direction =>
                {
                    Vector2 start = center + direction;
                    bool isMatch = IsWordMatch(input, "MAS", Convert.ToInt32(start.X), Convert.ToInt32(start.Y), -direction);
                    return Convert.ToInt32(isMatch);
                }) == 2; // we're looking for a cross
            });
    }

    private static bool IsWordMatch(char[,] buffer, ReadOnlySpan<char> searchWord, int x, int y, Vector2 direction)
    {
        int xLen = buffer.GetLength(0);
        int yLen = buffer.GetLength(1);

        foreach (char ch in searchWord)
        {
            if (x < 0 || x >= xLen || y < 0 || y >= yLen)
            {
                return false;
            }

            if (buffer[x, y] != ch)
            {
                return false;
            }

            x += Convert.ToInt32(direction.X);
            y += Convert.ToInt32(direction.Y);
        }

        return true;
    }
}
