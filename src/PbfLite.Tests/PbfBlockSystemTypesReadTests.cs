using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PbfLite.Tests {
    public class PbfBlockSystemTypesReadTests {
        [Theory]
        [InlineData(new byte[] { 0x0C, 0x45, 0x6E, 0x67, 0x6C, 0x69, 0x73, 0x68, 0x20, 0x74, 0x65, 0x78, 0x74 }, "English text")]
        [InlineData(new byte[] { 0x0C, 0xC4, 0x8C, 0x65, 0x73, 0x6B, 0xC3, 0xBD, 0x20, 0x74, 0x65, 0x78, 0x74 }, "Český text")]
        public void ReadString_ReadsUtf8EncodedString(byte[] data, string expectedText) {
            var block = PbfBlock.Create(data);

            var text = block.ReadString();

            Assert.Equal(expectedText, text);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, false)]
        [InlineData(new byte[] { 0x01 }, true)]
        public void ReadBoolean_ReadsValuesEncodedAsVarint(byte[] data, bool expectedValue) {
            var block = PbfBlock.Create(data);

            var value = block.ReadBoolean();

            Assert.Equal(expectedValue, value);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        [InlineData(new byte[] { 0x01 }, -1)]
        [InlineData(new byte[] { 0x02 }, 1)]
        [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, 2147483647)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, -2147483648)]
        private void ReadSignedInt_ReadsZiggedVarintValues(byte[] data, int expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadSignedInt();

            Assert.Equal(expectedNumber, number);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, 0)]
        [InlineData(new byte[] { 0x01 }, 1)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -1)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, 2147483647)]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xF0, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, -2147483648)]
        private void ReadInt_ReadsVarintValues(byte[] data, int expectedNumber) {
            var block = PbfBlock.Create(data);

            var number = block.ReadInt();

            Assert.Equal(expectedNumber, number);
        }
    }
}
