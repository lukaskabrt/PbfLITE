using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    [Theory]
    [InlineData(new byte[] { 0x08 }, 1)]
    [InlineData(new byte[] { 0x78 }, 15)]
    [InlineData(new byte[] { 0x80, 0x01 }, 16)]
    public void ReadFieldHeader_ReadsFieldNumbers(byte[] data, int fieldNumber)
    {
        var reader = PbfBlockReader.Create(data);

        var header = reader.ReadFieldHeader();

        Assert.Equal(fieldNumber, header.fieldNumber);
    }

    [Theory]
    [InlineData(new byte[] { 0x08 }, WireType.Varint)]
    [InlineData(new byte[] { 0x09 }, WireType.Fixed64)]
    [InlineData(new byte[] { 0x0A }, WireType.String)]
    [InlineData(new byte[] { 0x0D }, WireType.Fixed32)]
    public void ReadFieldHeader_ReadsWireTypes(byte[] data, WireType wireType)
    {
        var reader = PbfBlockReader.Create(data);

        var header = reader.ReadFieldHeader();

        Assert.Equal(wireType, header.wireType);
    }

    [Fact]
    public void ReadFieldHeader_ReturnsNoneWhenEndOfBlockIsReached()
    {
        var reader = PbfBlockReader.Create([]);

        var header = reader.ReadFieldHeader();

        Assert.Equal(0, header.fieldNumber);
        Assert.Equal(WireType.None, header.wireType);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, WireType.Varint, 1)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, WireType.Varint, 5)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, WireType.Varint, 10)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed32, 4)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed64, 8)]
    [InlineData(new byte[] { 0x03, 0x41, 0x42, 0x43 }, WireType.String, 4)]
    public void SkipFields_SkipsCorrectNumberOfBytes(byte[] data, WireType wireType, int expectedPosition)
    {
        var reader = PbfBlockReader.Create(data);

        reader.SkipField(wireType);

        Assert.Equal(expectedPosition, reader.Position);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(4294967294, 2147483647)]
    [InlineData(4294967295, -2147483648)]
    public void Zag_Decodes32BitZiggedValues(uint encodedNumber, int expectedNumber)
    {
        var number = PbfBlockReader.Zag(encodedNumber);

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
        var number = PbfBlockReader.Zag(encodedNumber);

        Assert.Equal(expectedNumber, number);
    }
}