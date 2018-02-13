using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PbfLite.Tests {
    public class PbfBufferTests {
        [Theory]
        [InlineData(1, new byte[] { 0x08 })]
        [InlineData(15, new byte[] { 0x78 })]
        [InlineData(16, new byte[] { 0x80, 0x01 })]
        public void WriteFieldHeader_WritesFieldNumbers(int fieldNumber, byte[] expectedData) {
            var buffer = PbfBuffer.Create(expectedData.Length);

            buffer.WriteFieldHeader(fieldNumber, WireType.Variant);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
        }

        [Theory]
        [InlineData(WireType.Variant, new byte[] { 0x08 })]
        [InlineData(WireType.Fixed64, new byte[] { 0x09 })]
        [InlineData(WireType.String, new byte[] { 0x0A })]
        [InlineData(WireType.Fixed32, new byte[] { 0x0D })]
        public void ReadFieldHeader_ReadsWireTypes(WireType wireType, byte[] expectedData) {
            var buffer = PbfBuffer.Create(expectedData.Length);

            buffer.WriteFieldHeader(1, wireType);

            var data = buffer.Buffer.ToArray();
            Assert.Equal(expectedData, data);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 2)]
        [InlineData(2147483647, 4294967294)]
        [InlineData(-2147483648, 4294967295)]
        public void Zig_Encodes32BitValues(int number, uint expected) {
            var encoded = PbfBuffer.Zig(number);

            Assert.Equal(expected, encoded);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(-1, 1)]
        [InlineData(1, 2)]
        [InlineData(9223372036854775807L, 18446744073709551614UL)]
        [InlineData(-9223372036854775808L, 18446744073709551615UL)]
        public void Zig_Encodes64BitValues(long number, ulong expected) {
            var encoded = PbfBuffer.Zig(number);

            Assert.Equal(expected, encoded);
        }
    }
}
