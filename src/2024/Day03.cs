using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Aoc;
using Aoc.NUnit;

namespace _2024;

public sealed class Day03 : Puzzle<IEnumerable<Day03.Operation>>
{
    protected override IEnumerable<Operation> GetInput(Stream stream)
    {
        return OperationParser.Parse(string.Join("", stream.ReadLines()));
    }

    [Solution(161)]
    public static int Part1(IEnumerable<Operation> operations)
    {
        return operations.Sum(op => op.Evaluate());
    }

    [Solution(48)]
    public static int Part2(IEnumerable<Operation> operations)
    {
        List<Node> list = [];
        Node? prev = null;
        foreach (Operation curr in operations)
        {
            var newNode = new Node(curr, prev);
            list.Add(newNode);
            prev = newNode;
        }

        return list
            .Where(n => n.IsEnabled)
            .Sum(n => n.Value.Evaluate());
    }

    // Not using a full-fledged tokenizer, we brute force some parts.
    private static class OperationParser
    {
        private static readonly Dictionary<string, Type> Parsers = new()
        {
            { Do.Token, typeof(Do) },
            { Dont.Token, typeof(Dont) },
            { Multiply.Token, typeof(Multiply) }
        };

        public static List<Operation> Parse(ReadOnlySpan<char> tokenizer)
        {
            List<Operation> operations = [];
            while (tokenizer.Length > 0)
            {
                Operation? operation = null;
                // Try parse each token at current cursor pos.
                foreach ((string token, Type parserType) in Parsers)
                {
                    if (!Operation.TryParse(token, parserType, tokenizer, out operation))
                    {
                        continue;
                    }

                    operations.Add(operation);
                    tokenizer = tokenizer[operation.ToString().Length..];
                    break;
                }

                // Forward cursor.
                if (operation is null)
                {
                    tokenizer = tokenizer[1..];
                }
            }

            return operations;
        }
    }

    public abstract record Operation
    {
        private readonly string _token;

        protected Operation(string token, int[] args)
        {
            _token = token;
            Args = args;
        }

        public int[] Args { get; }

        public virtual int Evaluate()
        {
            return 0;
        }

        public sealed override string ToString()
        {
            return $"{_token}({string.Join("", Args)})";
        }

        internal static bool TryParse(string token, Type type, ReadOnlySpan<char> tokenizer, [NotNullWhen(true)] out Operation? operation)
        {
            operation = null;
            if (tokenizer.StartsWith(token)
             && TryParseArgs(tokenizer[token.Length..], out List<int>? args))
            {
                operation = Activator.CreateInstance(type, args.Cast<object>().ToArray()) as Operation;
            }

            return operation is not null;
        }

        private static bool TryParseArgs(ReadOnlySpan<char> tokenizer, [NotNullWhen(true)] out List<int>? args)
        {
            if (!tokenizer.StartsWith('('))
            {
                args = null;
                return false;
            }

            tokenizer = tokenizer[1..];
            List<int> ops = [];
            while (tokenizer.Length > 0)
            {
                if (tokenizer.StartsWith(')'))
                {
                    break;
                }

                if (tokenizer.StartsWith(','))
                {
                    tokenizer = tokenizer[1..];
                }

                ReadOnlySpan<char> arg = tokenizer[..tokenizer.IndexOfAny(',', ')')];
                if (int.TryParse(arg, out int value))
                {
                    ops.Add(value);
                    tokenizer = tokenizer[arg.Length..];
                }
                else
                {
                    args = null;
                    return false;
                }
            }

            args = ops;
            return true;
        }
    }

    private sealed record Do() : Operation(Token, [])
    {
        public const string Token = "do";
    }

    // ReSharper disable once IdentifierTypo
    private sealed record Dont() : Operation(Token, [])
    {
        public const string Token = "don't";
    }

    private sealed record Multiply(int Left, int Right) : Operation(Token, [Left, Right])
    {
        public const string Token = "mul";

        public override int Evaluate()
        {
            return Left * Right;
        }
    }

    [DebuggerDisplay("{Value}, IsEnabled = {IsEnabled}")]
    private sealed record Node(Operation Value, Node? Previous)
    {
        public bool IsEnabled { get => Value is not Dont && (Value is Do || (Previous?.IsEnabled ?? true)); }
    };
}
