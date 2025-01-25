using Aoc;
using Aoc.NUnit;

namespace _2024;

public sealed class Day05 : Puzzle
{
    protected override object[] GetInput(Stream stream)
    {
        var lines = stream.ReadLines().ToList();
        var rules = lines.TakeWhile(s => !string.IsNullOrEmpty(s))
            .Select(s =>
            {
                int[] v = s.Split('|').Select(int.Parse).ToArray();
                return new Rule(v[0], v[1]);
            })
            .ToList();

        var updates = lines.SkipWhile(s => !string.IsNullOrEmpty(s))
            .Skip(1)
            .Select(s => new Update(s.Split(',').Select(int.Parse)))
            .ToList();

        return [rules, updates];
    }

    [Solution(143)]
    public static int Part1(List<Rule> rules, List<Update> updates)
    {
        return updates
            .Where(u => rules.All(rule => rule.IsValid(u)))
            .Sum(u => u[u.Count / 2]);
    }

    [Solution(123)]
    public static int Part2(List<Rule> rules, List<Update> updates)
    {
        var pageSorter = new PageSorter(rules);

        return updates
            .Where(u => !rules.All(rule => rule.IsValid(u)))
            .Sum(u =>
            {
                u.Sort(pageSorter);
                return u[u.Count / 2];
            });
    }

    public readonly record struct Rule(int Before, int After)
    {
        public bool IsValid(Update update)
        {
            int idxBefore = update.IndexOf(Before);
            int idxAfter = update.IndexOf(After);
            return idxBefore == -1 || idxAfter == -1 || idxBefore < idxAfter;
        }
    }

    public sealed class Update(IEnumerable<int> list) : List<int>(list);

    private sealed record PageSorter : IComparer<int>
    {
        private readonly Dictionary<int, HashSet<int>> _rules;

        public PageSorter(List<Rule> rules)
        {
            _rules = rules
                .GroupBy(r => r.Before)
                .ToDictionary(g => g.Key, g => g.Select(r => r.After).ToHashSet());
        }

        public int Compare(int x, int y)
        {
            if (x == y)
            {
                return 0;
            }

            if (_rules.TryGetValue(x, out HashSet<int>? after) && after.Contains(y))
            {
                return -1;
            }

            if (_rules.TryGetValue(y, out after) && after.Contains(x))
            {
                return 1;
            }

            return 0;
        }
    }
}
