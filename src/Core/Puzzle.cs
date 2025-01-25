namespace Aoc;

public abstract class Puzzle : Puzzle<object[]>
{
}

public abstract class Puzzle<TInput> : IPuzzle
{
    object? IPuzzle.GetInput(Stream stream)
    {
        return GetInput(stream);
    }

    protected abstract TInput GetInput(Stream stream);
}
