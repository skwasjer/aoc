using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Metadata;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace Aoc.NUnit;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public sealed class SolutionAttribute
    : NUnitAttribute, ISimpleTestBuilder, IApplyToTest, IImplyFixture
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
    public TestMethod BuildFrom(IMethodInfo method, Test? suite)
    {
        // Load user secrets with the known solutions.
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddUserSecrets(method.TypeInfo.Assembly)
            .Build();

        object input = GetPuzzleInput(method.TypeInfo.Type);
        object?[] testArgs = [input];

        // If type is object[], use it as test arguments instead (iow. multiple).
        Type inputType = input.GetType();
        if (inputType.IsArray && inputType.GetElementType() == typeof(object))
        {
            testArgs = (object?[])input;
        }

        object? expectedResult = GetExpectedPuzzleSolution(configuration, method.MethodInfo);
        TestCaseData testParams = new TestCaseData(testArgs) { ExpectedResult = expectedResult }
            .SetName($"{method.TypeInfo.Name}.{method.Name}(expects = {expectedResult?.ToString() ?? "<null>"})");
        return _builder.BuildTestMethod(method,
            suite,
            testParams
        );
    }

    private static object GetPuzzleInput(Type puzzleType)
    {
        if (!puzzleType.IsAssignableTo(typeof(IPuzzle)))
        {
            throw new InvalidOperationException($"Class must implement {typeof(IPuzzle).FullName}");
        }

        var puzzle = (IPuzzle)Activator.CreateInstance(puzzleType)!;
        MethodInfo getInputMethodType = puzzleType.GetMethod(nameof(IPuzzle.GetInput), BindingFlags.Instance | BindingFlags.NonPublic)!;
        return getInputMethodType.Invoke(puzzle, []) ?? throw new InvalidOperationException($"No puzzle input for puzzle type {puzzleType.FullName}.");
    }

    private object? GetExpectedPuzzleSolution(IConfiguration configuration, MethodInfo partMethod)
    {
        IConfigurationSection puzzleSolutions = configuration.GetSection(partMethod.DeclaringType!.Name.ToLowerInvariant());
        return puzzleSolutions.GetValue(partMethod.ReturnType, partMethod.Name.ToLowerInvariant()) ?? _expected;
    }
}
