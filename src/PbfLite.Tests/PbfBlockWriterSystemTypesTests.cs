using Xunit;

namespace PbfLite.Tests;

public class PbfBlockWriterSystemTypesTests
{
    [Theory]
    [InlineData("English text", new byte[] { 0x0C, 0x45, 0x6E, 0x67, 0x6C, 0x69, 0x73, 0x68, 0x20, 0x74, 0x65, 0x78, 0x74 })]
    [InlineData("Český text", new byte[] { 0x0C, 0xC4, 0x8C, 0x65, 0x73, 0x6B, 0xC3, 0xBD, 0x20, 0x74, 0x65, 0x78, 0x74 })]
    public void WriteString_WritesUtf8EncodedString(string text, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteString(text);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(false, new byte[] { 0x00 })]
    [InlineData(true, new byte[] { 0x01 })]
    public void WriteBoolean_WritesValuesEncodedAsVarint(bool value, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteBoolean(value);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00 })]
    [InlineData(-1, new byte[] { 0x01 })]
    [InlineData(1, new byte[] { 0x02 })]
    [InlineData(2147483647, new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F })]
    [InlineData(-2147483648, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
    public void WriteSignedInt_WritesZiggedVarintValues(int number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteSignedInt(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00 })]
    [InlineData(1, new byte[] { 0x01 })]
    [InlineData(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
    [InlineData(2147483647, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 })]
    [InlineData(-2147483648, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x08 })]
    public void WriteInt_WritesVarintValues(int number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteInt(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0u, new byte[] { 0x00 })]
    [InlineData(1u, new byte[] { 0x01 })]
    [InlineData(127u, new byte[] { 0x7F })]
    [InlineData(128u, new byte[] { 0x80, 0x01 })]
    [InlineData(4294967295u, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
    public void WriteUint_WritesVarintValues(uint number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteUint(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00 })]
    [InlineData(-1, new byte[] { 0x01 })]
    [InlineData(1, new byte[] { 0x02 })]
    [InlineData(9223372036854775807, new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
    [InlineData(-9223372036854775808, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
    public void WriteSignedLong_WritesZiggedVarintValues(long number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteSignedLong(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00 })]
    [InlineData(1, new byte[] { 0x01 })]
    [InlineData(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
    [InlineData(9223372036854775807, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F })]
    [InlineData(-9223372036854775808, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
    public void WriteLong_WritesVarintValues(long number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteLong(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0ul, new byte[] { 0x00 })]
    [InlineData(1ul, new byte[] { 0x01 })]
    [InlineData(127ul, new byte[] { 0x7F })]
    [InlineData(128ul, new byte[] { 0x80, 0x01 })]
    [InlineData(18446744073709551615ul, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
    public void WriteULongint_WritesVarintValues(ulong number, byte[] expectedData)
    {
        var buffer = new byte[expectedData.Length];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteULong(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
    [InlineData(3.14f, new byte[] { 0xC3, 0xF5, 0x48, 0x40 })]
    [InlineData(0.00014f, new byte[] { 0xF7, 0xCC, 0x12, 0x39 })]
    [InlineData(12000.1f, new byte[] { 0x66, 0x80, 0x3B, 0x46 })]
    public void WriteSingle_WritesValues(float number, byte[] expectedData)
    {
        var buffer = new byte[4];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteSingle(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }

    [Theory]
    [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
    [InlineData(3.14, new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 })]
    [InlineData(0.00014, new byte[] { 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F })]
    [InlineData(12000.1, new byte[] { 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 })]
    public void WriteDouble_WritesValues(double number, byte[] expectedData)
    {
        var buffer = new byte[8];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteDouble(number);

        SpanAssert.Equal(expectedData, writer.Block);
    }
}