using Aoc;
using Aoc.NUnit;
using static _2024.Day07;

namespace _2024;

public sealed class Day07 : Puzzle<List<MathProblem>>
{
    protected override List<MathProblem> GetInput(Stream stream)
    {
        return Parse().ToList();

        IEnumerable<MathProblem> Parse()
        {
            char[] splitOn = [':', ' '];
            foreach (string line in stream.ReadLines())
            {
                long[] arr = line.Split(splitOn, StringSplitOptions.RemoveEmptyEntries)
                    .Select(long.Parse)
                    .ToArray();
                yield return new MathProblem(arr[0], arr[1..]);
            }
        }
    }

    [Solution(3749)]
    public static long Part1(List<MathProblem> problems)
    {
        return problems
            .Where(p => EvaluateBranches(p.Operands,
                    (a, b) => a + b,
                    (a, b) => a * b
                )
                .Any(e => e == p.Result)
            )
            .Sum(p => p.Result);
    }

    [Solution(11387)]
    public static long Part2(List<MathProblem> problems)
    {
        return problems
            .Where(p => EvaluateBranches(p.Operands,
                    (a, b) => a + b,
                    (a, b) => a * b,
                    (a, b) => // OR
                    {
                        // Shift 'a' by nr of digits of b (in base 10) before adding both together.
                        int digits = (int)Math.Floor(Math.Log10(b) + 1);
                        return (a * (long)Math.Pow(10, digits)) + b;
                    })
                .Any(e => e == p.Result)
            )
            .Sum(p => p.Result);
    }

    private static IEnumerable<long> EvaluateBranches(long[] operands, params Func<long, long, long>[] evaluations)
    {
        IEnumerable<long> initialBranch = [operands[0], operands[0]];

        return operands
            .Skip(1)
            .Aggregate(initialBranch,
                // Create branches for each type of equation.
                (left, right) => left.SelectMany(e => evaluations.Select(fn => fn(e, right))));
    }

    public readonly record struct MathProblem(long Result, long[] Operands);
}
