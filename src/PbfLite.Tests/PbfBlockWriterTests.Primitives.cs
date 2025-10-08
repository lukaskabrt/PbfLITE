using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockWriterTests
{
    public class Primitives
    {
        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1, new byte[] { 0x01, 0x00, 0x00, 0x00 })]
        [InlineData(255, new byte[] { 0xFF, 0x00, 0x00, 0x00 })]
        [InlineData(256, new byte[] { 0x00, 0x01, 0x00, 0x00 })]
        [InlineData(65536, new byte[] { 0x00, 0x00, 0x01, 0x00 })]
        [InlineData(16777216, new byte[] { 0x00, 0x00, 0x00, 0x01 })]
        [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void WriteFixed32_WritesNumbers(uint number, byte[] expectedData)
        {
            var buffer = new byte[4];
            var writer = PbfBlockWriter.Create(buffer);

            writer.WriteFixed32(number);

            SpanAssert.Equal<byte>(expectedData, writer.Block);
        }

        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(255, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(18446744073709551615UL, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public void WriteFixed64_WritesNumbers(ulong number, byte[] expectedData)
        {
            var buffer = new byte[8];
            var writer = PbfBlockWriter.Create(buffer);

            writer.WriteFixed64(number);

            SpanAssert.Equal<byte>(expectedData, writer.Block);
        }

        [Theory]
        [InlineData(0, new byte[] { 0x00 })]
        [InlineData(1, new byte[] { 0x01 })]
        [InlineData(127, new byte[] { 0x7F })]
        [InlineData(128, new byte[] { 0x80, 0x01 })]
        [InlineData(16384, new byte[] { 0x80, 0x80, 0x01 })]
        [InlineData(2097152, new byte[] { 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(268435456, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void WriteVarint32_WritesNumbers(uint number, byte[] expectedData)
        {
            // Maximum 5 bytes for a 32-bit varint
            var buffer = new byte[5];
            var writer = PbfBlockWriter.Create(buffer);

            writer.WriteVarInt32(number);

            SpanAssert.Equal<byte>(expectedData, writer.Block);
        }

        [Theory]
        [InlineData(0, new byte[] { 0x00 })]
        [InlineData(1, new byte[] { 0x01 })]
        [InlineData(127, new byte[] { 0x7F })]
        [InlineData(128, new byte[] { 0x80, 0x01 })]
        [InlineData(16384, new byte[] { 0x80, 0x80, 0x01 })]
        [InlineData(2097152, new byte[] { 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(268435456, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(34359738368UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(4398046511104UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(562949953421312UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(72057594037927936UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(18446744073709551615UL, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteVarint64_WritesNumbers(ulong number, byte[] expectedData)
        {
            // Maximum 10 bytes for a 64-bit varint
            var buffer = new byte[10];
            var writer = PbfBlockWriter.Create(buffer);

            writer.WriteVarInt64(number);

            SpanAssert.Equal<byte>(expectedData, writer.Block);
        }

        [Fact]
        public void WriteLengthPrefixedBytes_WritesPrefixAndData()
        {
            var data = new byte[] { 0x41, 0x42, 0x43 };
            var expected = new byte[] { 0x03, 0x41, 0x42, 0x43 };

            var buffer = new byte[4];
            var writer = PbfBlockWriter.Create(buffer);

            writer.WriteLengthPrefixedBytes(data);

            SpanAssert.Equal<byte>(expected, writer.Block);
        }
    }
}