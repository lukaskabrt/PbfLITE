using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests : PbfReaderTests
{
    public override (int fieldNumber, WireType wireType) ReadFieldHeader(byte[] data)
    {
        var reader = PbfBlockReader.Create(data);
        return reader.ReadFieldHeader();
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, WireType.VarInt, 1)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, WireType.VarInt, 5)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, WireType.VarInt, 10)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed32, 4)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed64, 8)]
    [InlineData(new byte[] { 0x03, 0x41, 0x42, 0x43 }, WireType.String, 4)]
    public void SkipFields_SkipsCorrectNumberOfBytes(byte[] data, WireType wireType, int expectedPosition)
    {
        var reader = PbfBlockReader.Create(data);

        reader.SkipField(wireType);

        Assert.Equal(expectedPosition, reader.Position);
    }
}
