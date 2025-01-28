namespace Aoc;

public static class StreamExtensions
{
    public static IEnumerable<string> ReadLines(this Stream stream)
    {
        if (stream is null)
        {
            throw new ArgumentNullException(nameof(stream));
        }

        return ReadLinesCore().ToList();

        IEnumerable<string> ReadLinesCore()
        {
            using (stream)
            {
                using var sr = new StreamReader(stream);
                while (sr.ReadLine() is { } line)
                {
                    yield return line;
                }
            }
        }
    }

    public static char[,] Read2DArray(this Stream stream)
    {
        return stream.Read2DArray(ch => ch);
    }

    public static T[,] Read2DArray<T>(this Stream stream, Func<char, T> parse)
    {
        return stream.Read2DArray((ch, _, _) => parse(ch));
    }

    public static T[,] Read2DArray<T>(this Stream stream, Func<char, int, int, T> parse)
    {
        IEnumerable<string> input = stream.ReadLines();

        string[] data = input.ToArray();
        var buffer = new T[data[0].Length, data.Length];

        for (int y = 0; y < data.Length; y++)
        {
            string line = data[y];
            for (int x = 0; x < line.Length; x++)
            {
                buffer[x, y] = parse(line[x], x, y);
            }
        }

        return buffer;
    }
}
