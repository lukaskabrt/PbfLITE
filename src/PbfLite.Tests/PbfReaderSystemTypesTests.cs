using System;
using Xunit;

namespace PbfLite.Tests;

public abstract class PbfReaderSystemTypesTests
{
    public abstract string ReadString(byte[] data);

    public abstract bool ReadBoolean(byte[] data);

    public abstract int ReadSignedInt(byte[] data);

    public abstract int ReadInt(byte[] data);

    public abstract uint ReadUint(byte[] data);

    public abstract long ReadSignedLong(byte[] data);

    public abstract long ReadLong(byte[] data);

    public abstract ulong ReadULong(byte[] data);

    public abstract float ReadSingle(byte[] data);

    public abstract double ReadDouble(byte[] data);

    [Theory]
    [InlineData(new byte[] { 0x0C, 0x45, 0x6E, 0x67, 0x6C, 0x69, 0x73, 0x68, 0x20, 0x74, 0x65, 0x78, 0x74 }, "English text")]
    [InlineData(new byte[] { 0x0C, 0xC4, 0x8C, 0x65, 0x73, 0x6B, 0xC3, 0xBD, 0x20, 0x74, 0x65, 0x78, 0x74 }, "Český text")]
    public void ReadString_ReadsUtf8EncodedString(byte[] data, string expectedText)
    {
        var text = ReadString(data);

        Assert.Equal(expectedText, text);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, false)]
    [InlineData(new byte[] { 0x01 }, true)]
    public void ReadBoolean_ReadsValuesEncodedAsVarint(byte[] data, bool expectedValue)
    {
        var value = ReadBoolean(data);

        Assert.Equal(expectedValue, value);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0)]
    [InlineData(new byte[] { 0x01 }, -1)]
    [InlineData(new byte[] { 0x02 }, 1)]
    [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, 2147483647)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, -2147483648)]
    public void ReadSignedInt_ReadsZiggedVarintValues(byte[] data, int expectedNumber)
    {
        var number = ReadSignedInt(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0)]
    [InlineData(new byte[] { 0x01 }, 1)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -1)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, 2147483647)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -2147483648)]
    public void ReadInt_ReadsVarintValues(byte[] data, int expectedNumber)
    {
        var number = ReadInt(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0U)]
    [InlineData(new byte[] { 0x01 }, 1U)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, 4294967295U)]
    public void ReadUint_ReadsVarintValues(byte[] data, uint expectedNumber)
    {
        var number = ReadUint(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0)]
    [InlineData(new byte[] { 0x01 }, -1)]
    [InlineData(new byte[] { 0x02 }, 1)]
    [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, 9223372036854775807)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -9223372036854775808)]
    public void ReadSignedLong_ReadsZiggedVarintValues(byte[] data, long expectedNumber)
    {
        var number = ReadSignedLong(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0L)]
    [InlineData(new byte[] { 0x01 }, 1L)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -1L)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, 9223372036854775807)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, -9223372036854775808)]
    public void ReadLong_ReadsVarintValues(byte[] data, long expectedNumber)
    {
        var number = ReadLong(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0UL)]
    [InlineData(new byte[] { 0x01 }, 1UL)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, 18446744073709551615UL)]
    public void ReadULong_ReadsVarintValues(byte[] data, ulong expectedNumber)
    {
        var number = ReadULong(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0f)]
    [InlineData(new byte[] { 0xC3, 0xF5, 0x48, 0x40 }, 3.14f)]
    [InlineData(new byte[] { 0xF7, 0xCC, 0x12, 0x39 }, 0.00014f)]
    [InlineData(new byte[] { 0x66, 0x80, 0x3B, 0x46 }, 12000.1f)]
    public void ReadSingle_ReadsValues(byte[] data, float expectedNumber)
    {
        var number = ReadSingle(data);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0d)]
    [InlineData(new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, 3.14)]
    [InlineData(new byte[] { 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F }, 0.00014)]
    [InlineData(new byte[] { 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, 12000.1)]
    public void ReadDouble_ReadsValues(byte[] data, double expectedNumber)
    {
        var number = ReadDouble(data);

        Assert.Equal(expectedNumber, number);
    }
}
