namespace Aoc;

public static class PuzzleExtensions
{
    public static IEnumerable<string> ReadLines(this IPuzzle puzzle)
    {
        if (puzzle is null)
        {
            throw new ArgumentNullException(nameof(puzzle));
        }

        string puzzleName = puzzle.GetType().Name.ToLowerInvariant();
        string e2eFileName = $"inputs/e2e/{puzzleName}.txt";
        return File.ReadAllLines(File.Exists(e2eFileName)
            ? e2eFileName
            : $"inputs/{puzzleName}.txt"
        );
    }

    public static char[,] Read2DArray(this IPuzzle puzzle)
    {
        IEnumerable<string> input = puzzle.ReadLines();

        string[] data = input.ToArray();
        char[,] buffer = new char[data[0].Length, data.Length];

        for (int y = 0; y < data.Length; y++)
        {
            string line = data[y];
            for (int x = 0; x < line.Length; x++)
            {
                buffer[x, y] = line[x];
            }
        }

        return buffer;
    }
}
