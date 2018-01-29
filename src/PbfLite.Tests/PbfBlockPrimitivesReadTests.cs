using System;
using Xunit;

namespace PbfLite.Tests {
    public class PbfBlockPrimitivesReadTests {
        [Theory]
        [InlineData(new byte[] { 0x08 }, 1)]
        [InlineData(new byte[] { 0x78 }, 15)]
        [InlineData(new byte[] { 0x80, 0x01 }, 16)]
        public void ReadFieldHeader_ReadsFieldNumbers(byte[] data, int fieldNumber) {
            var block = PbfBlock.Create(data);

            var header = block.ReadFieldHeader();

            Assert.Equal(fieldNumber, header.fieldNumber);
        }

        [Theory]
        [InlineData(new byte[] { 0x08 }, WireType.Variant)]
        [InlineData(new byte[] { 0x09 }, WireType.Fixed64)]
        [InlineData(new byte[] { 0x0A }, WireType.String)]
        [InlineData(new byte[] { 0x0D }, WireType.Fixed32)]
        public void ReadFieldHeader_ReadsWireTypes(byte[] data, WireType wireType) {
            var block = PbfBlock.Create(data);

            var header = block.ReadFieldHeader();

            Assert.Equal(wireType, header.wireType);
        }

        [Fact]
        public void ReadFieldHeader_ReturnsNoneWhenEndOfBlockIsReached() {
            var block = PbfBlock.Create(new byte[] { });

            var header = block.ReadFieldHeader();

            Assert.Equal(0, header.fieldNumber);
            Assert.Equal(WireType.None, header.wireType);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0)]
        [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00 }, 1)]
        [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 255)]
        [InlineData(new byte[] { 0x00, 0x01, 0x00, 0x00 }, 256)]
        [InlineData(new byte[] { 0x00, 0x00, 0x01, 0x00 }, 65536)]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 16777216)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 4294967295)]
        public void ReadFixed32_ReadsNumbers(byte[] data, uint expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadFixed32();

            Assert.Equal(expectedNumber, number);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0)]
        [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1)]
        [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 255)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 }, 4294967295)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 18446744073709551615UL)]
        public void ReadFixed64_ReadsNumbers(byte[] data, ulong expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadFixed64();

            Assert.Equal(expectedNumber, number);
        }

        [Fact]
        public void ReadLengthPrefixedBytes_ReadsData() {
            var block = PbfBlock.Create(new byte[] { 0x03, 0x41, 0x42, 0x43 });

            var data = block.ReadLengthPrefixedBytes();

            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, data.ToArray());
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
        public void ReadVarint32_ReadsNumbers(byte[] data, uint expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadVarint32();

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
        public void ReadVarint64_ReadsNumbers(byte[] data, ulong expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadVarint64();

            Assert.Equal(expectedNumber, number);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(2, 1)]
        [InlineData(4294967294, 2147483647)]
        [InlineData(4294967295, -2147483648)]
        private void Zag_Decodes32BitZiggedValues(uint encodedNumber, int expectedNumber) {
            var block = PbfBlock.Create(new byte[] { });

            var number = block.Zag(encodedNumber);

            Assert.Equal(expectedNumber, number);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(2, 1)]
        [InlineData(18446744073709551614UL, 9223372036854775807L)]
        [InlineData(18446744073709551615UL, -9223372036854775808L)]
        private void Zag_Decodes64BitZiggedValues(ulong encodedNumber, long expectedNumber) {
            var block = PbfBlock.Create(new byte[] { });

            var number = block.Zag(encodedNumber);

            Assert.Equal(expectedNumber, number);
        }
    }
}
