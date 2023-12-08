namespace Aoc;

public abstract class Puzzle : Puzzle<object[]>
{
}

public abstract class Puzzle<TInput> : IPuzzle
{
    object? IPuzzle.GetInput()
    {
        return GetInput();
    }

    protected abstract TInput GetInput();

    protected IEnumerable<string> ReadInput()
    {
        string puzzleName = GetType().Name.ToLowerInvariant();
        string e2eFileName = $"inputs/e2e/{puzzleName}.txt";
        return File.ReadAllLines(File.Exists(e2eFileName)
            ? e2eFileName
            : $"inputs/{puzzleName}.txt"
        );
    }
}
