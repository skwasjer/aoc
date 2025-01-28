using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Aoc;

public static class ArrayExtensions
{
    public static IEnumerable<(int x, int y)> FindPositions<T>(this T[,] buffer, T find)
        where T : IEquatable<T>
    {
        return buffer.FindPositions(v => v.Equals(find));
    }

    public static IEnumerable<(int x, int y)> FindPositions<T>(this T[,] buffer, Func<T, bool> predicate)
        where T : IEquatable<T>
    {
        int xLen = buffer.GetLength(0);
        int yLen = buffer.GetLength(1);

        for (int y = 0; y < yLen; y++)
        {
            for (int x = 0; x < xLen; x++)
            {
                T ch = buffer[x, y];
                if (!predicate(ch))
                {
                    continue;
                }

                yield return (x, y);
            }
        }
    }

    public static IEnumerable<((int x, int y) pos, T value)> GetAll<T>(this T[,] buffer)
    {
        int xLen = buffer.GetLength(0);
        int yLen = buffer.GetLength(1);

        for (int y = 0; y < yLen; y++)
        {
            for (int x = 0; x < xLen; x++)
            {
                yield return ((x, y), buffer[x, y]);
            }
        }
    }

    public static bool TryGetAt<T>(this T[,] buffer, Vector2 pos, [NotNullWhen(true)] out T? value)
        where T : struct
    {
        int xLen = buffer.GetLength(0);
        int yLen = buffer.GetLength(1);

        int x = Convert.ToInt32(pos.X);
        int y = Convert.ToInt32(pos.Y);

        if (x < 0 || x >= xLen || y < 0 || y >= yLen)
        {
            value = null;
            return false;
        }

        value = buffer[x, y];
        return true;
    }
}
