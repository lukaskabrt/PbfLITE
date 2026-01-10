using Xunit;

namespace PbfLite.Tests;

public abstract partial class PbfReaderTests
{
    public abstract (int fieldNumber, WireType wireType) ReadFieldHeader(byte[] data);

    [Theory]
    [InlineData(new byte[] { 0x08 }, 1)]
    [InlineData(new byte[] { 0x78 }, 15)]
    [InlineData(new byte[] { 0x80, 0x01 }, 16)]
    public void ReadFieldHeader_ReadsFieldNumbers(byte[] data, int fieldNumber)
    {
        var header = ReadFieldHeader(data);

        Assert.Equal(fieldNumber, header.fieldNumber);
    }

    [Theory]
    [InlineData(new byte[] { 0x08 }, WireType.VarInt)]
    [InlineData(new byte[] { 0x09 }, WireType.Fixed64)]
    [InlineData(new byte[] { 0x0A }, WireType.String)]
    [InlineData(new byte[] { 0x0D }, WireType.Fixed32)]
    public void ReadFieldHeader_ReadsWireTypes(byte[] data, WireType wireType)
    {
        var header = ReadFieldHeader(data);

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
}
