﻿using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PbfLite.Tests {
    public class PbfBlockTests {
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
        [InlineData(new byte[] { 0x00 }, WireType.Variant, 1)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, WireType.Variant, 5)]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, WireType.Variant, 10)]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed32, 4)]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed64, 8)]
        [InlineData(new byte[] { 0x03, 0x41, 0x42, 0x43 }, WireType.String, 4)]
        public void SkipFields_SkipsCorrectNumberOfBytes(byte[] data, WireType wireType, int expectedPosition) {
            var block = PbfBlock.Create(data);

            block.SkipField(wireType);

            Assert.Equal(expectedPosition, block.Position);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(2, 1)]
        [InlineData(4294967294, 2147483647)]
        [InlineData(4294967295, -2147483648)]
        public void Zag_Decodes32BitZiggedValues(uint encodedNumber, int expectedNumber) {
            var number = PbfBlock.Zag(encodedNumber);

            Assert.Equal(expectedNumber, number);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(1, -1)]
        [InlineData(2, 1)]
        [InlineData(18446744073709551614UL, 9223372036854775807L)]
        [InlineData(18446744073709551615UL, -9223372036854775808L)]
        public void Zag_Decodes64BitZiggedValues(ulong encodedNumber, long expectedNumber) {
            var number = PbfBlock.Zag(encodedNumber);

            Assert.Equal(expectedNumber, number);
        }
    }
}
