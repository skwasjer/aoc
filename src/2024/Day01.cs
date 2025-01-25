using Aoc;
using Aoc.NUnit;

namespace _2024;

public sealed class Day01 : Puzzle
{
    protected override object[] GetInput(Stream stream)
    {
        List<int> left = [];
        List<int> right = [];
        foreach (string[] values in stream.ReadLines()
                     .Select(s => s.Split(' ', StringSplitOptions.RemoveEmptyEntries)))
        {
            left.Add(int.Parse(values[0]));
            right.Add(int.Parse(values[1]));
        }

        return [left, right];
    }

    [Solution(11)]
    public static int Part1(List<int> left, List<int> right)
    {
        left.Sort();
        right.Sort();

        return left
            .Zip(right)
            .Sum(t => Math.Abs(t.First - t.Second));
    }

    [Solution(31)]
    public static int Part2(List<int> left, List<int> right)
    {
        return left.Sum(l => l * right.Count(r => r == l));
    }
}
