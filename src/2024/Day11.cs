using System.Collections.Concurrent;
using Aoc;
using Aoc.NUnit;
using static _2024.Day11;

namespace _2024;

public sealed class Day11 : Puzzle<Stone[]>
{
    protected override Stone[] GetInput(Stream stream)
    {
        return stream.ReadLines()
            .FirstOrDefault()
            ?.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => new Stone(long.Parse(s)))
            .ToArray() ?? [];
    }

    [Solution(55312)]
    public static long Part1(Stone[] stones)
    {
        return stones.Sum(s => s.Blink(25));
    }

    [Solution(65601038650482)]
    public static long Part2(Stone[] stones)
    {
        return stones.Sum(s => s.Blink(75));
    }

    public record Stone(long Value)
    {
        private static readonly ConcurrentDictionary<(long value, int iterations), long> Cache = [];

        public long Blink(int iterations)
        {
            return CountSplits(Value, iterations) + 1;
        }

        private static long CountSplits(long value, int iterations)
        {
            if (Cache.TryGetValue((value, iterations), out long splitCount))
            {
                return splitCount;
            }

            int i = iterations;
            long v = value;
            while (i-- > 0)
            {
                if (v == 0)
                {
                    v = 1;
                    continue;
                }

                int digits = (int)Math.Floor(Math.Log10(v) + 1);
                if (digits % 2 == 0)
                {
                    long f = (long)Math.Pow(10, (double)digits / 2);
                    long hi = v / f;
                    long lo = v % f;
                    v = hi;
                    splitCount += 1 + CountSplits(lo, i);
                }
                else
                {
                    v *= 2024;
                }
            }

            Cache[(value, iterations)] = splitCount;
            return splitCount;
        }
    }
}
