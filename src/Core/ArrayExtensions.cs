namespace Aoc;

public static class ArrayExtensions
{
    public static IEnumerable<(int x, int y)> FindPositions<T>(this T[,] buffer, T find)
        where T : IEquatable<T>
    {
        int xLen = buffer.GetLength(0);
        int yLen = buffer.GetLength(1);

        for (int y = 0; y < yLen; y++)
        {
            for (int x = 0; x < xLen; x++)
            {
                T ch = buffer[x, y];
                if (!ch.Equals(find))
                {
                    continue;
                }

                yield return (x, y);
            }
        }
    }
}
