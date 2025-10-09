using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockWriterTests
{
    [Theory]
    [InlineData(1, WireType.VarInt, new byte[] { 0x08 })]
    [InlineData(1, WireType.Fixed64, new byte[] { 0x09 })]
    [InlineData(1, WireType.String, new byte[] { 0x0A })]
    [InlineData(1, WireType.Fixed32, new byte[] { 0x0D })]
    [InlineData(16, WireType.VarInt, new byte[] { 0x80, 0x01 })]
    public void WriteFieldHeader_WritesCorrectBytes(int fieldNumber, WireType wireType, byte[] expectedBytes)
    {
        var writer = PbfBlockWriter.Create(new byte[2]);

        writer.WriteFieldHeader(fieldNumber, wireType);

        SpanAssert.Equal<byte>(expectedBytes, writer.Block);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 2)]
    [InlineData(2147483647, 4294967294)]
    [InlineData(-2147483648, 4294967295)]
    public void Zig_Encodes32BitValues(int number, uint expectedEncodedNumber)
    {
        var encodedNumber = PbfBlockWriter.Zig(number);

        Assert.Equal(expectedEncodedNumber, encodedNumber);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 1)]
    [InlineData(1, 2)]
    [InlineData(9223372036854775807L, 18446744073709551614UL)]
    [InlineData(-9223372036854775808L, 18446744073709551615UL)]
    public void Zig_Encodes64BitValues(long number, ulong expectedEncodedNumber)
    {
        var encodedNumber = PbfBlockWriter.Zig(number);

        Assert.Equal(expectedEncodedNumber, encodedNumber);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(127, 1)]
    [InlineData(128, 2)]
    [InlineData(16383, 2)]
    [InlineData(16384, 3)]
    [InlineData(2097151, 3)]
    [InlineData(2097152, 4)]
    [InlineData(268435455, 4)]
    [InlineData(268435456, 5)]
    [InlineData(uint.MaxValue, 5)]
    public void GetVarIntByteCount_BorderValues(uint value, int expectedBytesCount)
    {
        var bytesCount = PbfBlockWriter.GetVarIntBytesCount(value);

        Assert.Equal(expectedBytesCount, bytesCount);
    }
}