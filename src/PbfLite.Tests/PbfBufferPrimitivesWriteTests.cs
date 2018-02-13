using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PbfLite.Tests {
    public class PbfBufferPrimitivesWriteTests {
        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1, new byte[] { 0x01, 0x00, 0x00, 0x00 })]
        [InlineData(255, new byte[] { 0xFF, 0x00, 0x00, 0x00 })]
        [InlineData(256, new byte[] { 0x00, 0x01, 0x00, 0x00 })]
        [InlineData(65536, new byte[] { 0x00, 0x00, 0x01, 0x00 })]
        [InlineData(16777216, new byte[] { 0x00, 0x00, 0x00, 0x01 })]
        [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
        public void WriteFixed32_WritesNumbers(uint number, byte[]  expectedData) {
            var buffer = PbfBuffer.Create(4);

            buffer.WriteFixed32(number);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
        }

        [Theory]
        [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(1, new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(255, new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 })]
        [InlineData(18446744073709551615UL, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF })]
        public void WriteFixed64_WritesNumbers(ulong number, byte[] expectedData) {
            var buffer = PbfBuffer.Create(8);

            buffer.WriteFixed64(number);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
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
        [InlineData(34359738368UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(4398046511104UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(562949953421312UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(72057594037927936UL, new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        [InlineData(18446744073709551615UL, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteVarint64_WritesNumbers(ulong number, byte[] expectedData) {
            var buffer = PbfBuffer.Create(10);

            buffer.WriteVarint(number);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
        }

        [Theory]
        [InlineData(new byte[] { 0x41, 0x42, 0x43 }, new byte[] { 0x03, 0x41, 0x42, 0x43 })]
        public void WriteLengthPrefixedBytes_WritesData(byte[] bytes, byte[] expectedData) {
            var buffer = PbfBuffer.Create(expectedData.Length);

            buffer.WriteLengthPrefixedBytes(bytes);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
        }
    }
}
