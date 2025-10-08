using Xunit;

namespace PbfLite.Tests;

public class PbfBlockReaderPrimitivesTests
{
    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00 }, 1)]
    [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 255)]
    [InlineData(new byte[] { 0x00, 0x01, 0x00, 0x00 }, 256)]
    [InlineData(new byte[] { 0x00, 0x00, 0x01, 0x00 }, 65536)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 16777216)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 4294967295)]
    public void ReadFixed32_ReadsNumbers(byte[] data, uint expectedNumber)
    {
        var reader = PbfBlockReader.Create(data);

        var number = reader.ReadFixed32();

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1)]
    [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 255)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 }, 4294967295)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 18446744073709551615UL)]
    public void ReadFixed64_ReadsNumbers(byte[] data, ulong expectedNumber)
    {
        var reader = PbfBlockReader.Create(data);

        var number = reader.ReadFixed64();

        Assert.Equal(expectedNumber, number);
    }

    [Fact]
    public void ReadLengthPrefixedBytes_ReadsData()
    {
        var reader = PbfBlockReader.Create([0x03, 0x41, 0x42, 0x43]);

        var data = reader.ReadLengthPrefixedBytes();

        Assert.Equal([0x41, 0x42, 0x43], data.ToArray());
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0)]
    [InlineData(new byte[] { 0x01 }, 1)]
    [InlineData(new byte[] { 0x7F }, 127)]
    [InlineData(new byte[] { 0x80, 0x01 }, 128)]
    [InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, 4294967295)]
    public void ReadVarint32_ReadsNumbers(byte[] data, uint expectedNumber)
    {
        var reader = PbfBlockReader.Create(data);

        var number = reader.ReadVarInt32();

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0)]
    [InlineData(new byte[] { 0x01 }, 1)]
    [InlineData(new byte[] { 0x7F }, 127)]
    [InlineData(new byte[] { 0x80, 0x01 }, 128)]
    [InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 34359738368UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 4398046511104UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 562949953421312UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 72057594037927936UL)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, 18446744073709551615UL)]
    public void ReadVarint64_ReadsNumbers(byte[] data, ulong expectedNumber)
    {
        var reader = PbfBlockReader.Create(data);

        var number = reader.ReadVarInt64();

        Assert.Equal(expectedNumber, number);
    }
}