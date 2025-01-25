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
}
