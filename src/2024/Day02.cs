using Aoc;
using Aoc.NUnit;

namespace _2024;

public sealed class Day02 : Puzzle<IEnumerable<Day02.Report>>
{
    private const int MaxDelta = 3;

    protected override IEnumerable<Report> GetInput()
    {
        return ReadInput()
            .Select(s =>
                new Report(
                    s.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                )
            );
    }

    [Solution(2)]
    public static int Part1(IEnumerable<Report> reports)
    {
        return reports.Count(r => r.IsSafe());
    }

    [Solution(4)]
    public static int Part2(IEnumerable<Report> reports)
    {
        return reports.Count(r => r.IsSafe(true));
    }

    public record Report
    {
        public Report(IEnumerable<int> levels)
        {
            Levels = new LinkedList<int>(levels);
        }

        public LinkedList<int> Levels { get; }

        public bool IsSafe(bool withDampening = false)
        {
            return IsSafe(Levels, withDampening);
        }

        private static bool IsSafe(LinkedList<int> report)
        {
            LinkedListNode<int> curr = report.First!;
            int direction = Math.Sign(curr.Next!.Value - curr.Value);

            while (curr.Next is not null)
            {
                LinkedListNode<int> next = curr.Next;
                if (!IsSafeLevel(curr, next, direction))
                {
                    return false;
                }

                curr = next;
            }

            return true;
        }

        private static bool IsSafe(LinkedList<int> report, bool withDampening)
        {
            if (IsSafe(report))
            {
                return true;
            }

            if (!withDampening)
            {
                return false;
            }

            LinkedListNode<int>? curr = report.First;

            while (curr is not null)
            {
                LinkedListNode<int>? prev = curr.Previous;
                LinkedListNode<int>? next = curr.Next;

                // Temp remove node.
                report.Remove(curr);
                if (IsSafe(report))
                {
                    return true;
                }

                // Add node back.
                if (prev is not null)
                {
                    report.AddAfter(prev, curr);
                }
                else if (next is not null)
                {
                    report.AddBefore(next, curr);
                }

                curr = next;
            }

            return false;
        }

        private static bool IsSafeLevel(LinkedListNode<int> curr, LinkedListNode<int> next, int direction)
        {
            int delta = next.Value - curr.Value;
            return Math.Abs(delta) <= MaxDelta && Math.Sign(delta) == direction;
        }
    }
}
