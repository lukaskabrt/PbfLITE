using System;
using Xunit;

namespace PbfLite.Tests;

internal static class SpanAssert
{
    public static void Equal<T>(ReadOnlySpan<T> expected, ReadOnlySpan<T> actual)
    {
        Assert.True(expected.SequenceEqual(actual),
            $"Expected: [{string.Join(", ", expected.ToArray())}], Actual: [{string.Join(", ", actual.ToArray())}]");
    }
}
