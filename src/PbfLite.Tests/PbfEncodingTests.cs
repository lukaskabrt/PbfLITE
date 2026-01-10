using Xunit;

namespace PbfLite.Tests;

public class PbfEncodingTests
{
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(4294967294, 2147483647)]
    [InlineData(4294967295, -2147483648)]
    public void Zag_Decodes32BitZiggedValues(uint encodedNumber, int expectedNumber)
    {
        var number = PbfEncoding.Zag(encodedNumber);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(18446744073709551614UL, 9223372036854775807L)]
    [InlineData(18446744073709551615UL, -9223372036854775808L)]
    public void Zag_Decodes64BitZiggedValues(ulong encodedNumber, long expectedNumber)
    {
        var number = PbfEncoding.Zag(encodedNumber);

        Assert.Equal(expectedNumber, number);
    }
}