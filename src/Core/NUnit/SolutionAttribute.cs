using System.Reflection;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Aoc.NUnit;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class SolutionAttribute
    : NUnitAttribute, ITestBuilder, IApplyToTest, IImplyFixture
{
    private readonly NUnitTestCaseBuilder _builder = new();
    private readonly object? _expected;

    public SolutionAttribute(object? expected = null)
    {
        _expected = expected;
    }

    /// <inheritdoc />
    public void ApplyToTest(Test test)
    {
    }

    /// <inheritdoc />
    public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test? suite)
    {
        Type puzzleType = method.TypeInfo.Type;
        string puzzleName = puzzleType.Name.ToLowerInvariant();

        string exampleFilename = $"inputs/{puzzleName}.txt";
        yield return BuildFrom(method, suite, "example", File.Exists(exampleFilename) ? File.OpenRead(exampleFilename) : Stream.Null, _expected);

        string e2eFilename = $"inputs/e2e/{puzzleName}.txt";
        if (File.Exists(e2eFilename))
        {
            yield return BuildFrom(method, suite, "aoc", File.OpenRead(e2eFilename), GetSecretPuzzleSolution(method));
        }
    }

    private TestMethod BuildFrom(IMethodInfo method, Test? suite, string tag, Stream inputData, object? expectedResult)
    {
        object input = GetPuzzleInput(method.TypeInfo.Type, inputData);
        object?[] testArgs = [input];

        // If type is object[], use it as test arguments instead (iow. multiple).
        Type inputType = input.GetType();
        if (inputType.IsArray && inputType.GetElementType() == typeof(object))
        {
            testArgs = (object?[])input;
        }

        TestCaseData testParams = new TestCaseData(testArgs) { ExpectedResult = expectedResult }
            .SetName($"{method.TypeInfo.Name}, {method.Name}({tag}): {expectedResult?.ToString() ?? "<null>"}");
        TestMethod testMethod = _builder.BuildTestMethod(method,
            suite,
            testParams
        );
        testMethod.ApplyAttributesToTest([new CategoryAttribute(tag)]);
        return testMethod;
    }

    private static object GetPuzzleInput(Type puzzleType, Stream stream)
    {
        if (!puzzleType.IsAssignableTo(typeof(IPuzzle)))
        {
            throw new InvalidOperationException($"Class must implement {typeof(IPuzzle).FullName}");
        }

        var puzzle = (IPuzzle)Activator.CreateInstance(puzzleType)!;
        MethodInfo getInputMethodType = puzzleType.GetMethod(nameof(IPuzzle.GetInput), BindingFlags.Instance | BindingFlags.NonPublic)!;
        return getInputMethodType.Invoke(puzzle, [stream]) ?? throw new InvalidOperationException($"No puzzle input for puzzle type {puzzleType.FullName}.");
    }

    private static object? GetSecretPuzzleSolution(IMethodInfo method)
    {
        // Load user secrets with the known solutions.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddUserSecrets(method.TypeInfo.Assembly)
            .Build();

        MethodInfo partMethod = method.MethodInfo;
        IConfigurationSection puzzleSolutions = configuration.GetSection(partMethod.DeclaringType!.Name.ToLowerInvariant());
        return puzzleSolutions.GetValue(partMethod.ReturnType, partMethod.Name.ToLowerInvariant());
    }
}
