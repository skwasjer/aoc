using NUnit.Framework;

namespace Aoc.NUnit;

internal static class NUnitLoader
{
    static NUnitLoader()
    {
        // Force load of NUnit so test adapter can resolve all custom puzzle tests.
        _ = new TestAttribute();
    }
}
