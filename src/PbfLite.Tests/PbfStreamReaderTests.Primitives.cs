using System;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public class PbfStreamReaderTests_Primitives
{
    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0U)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00 }, 1U)]
    [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00 }, 255U)]
    [InlineData(new byte[] { 0x00, 0x01, 0x00, 0x00 }, 256U)]
    [InlineData(new byte[] { 0x00, 0x00, 0x01, 0x00 }, 65536U)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x01 }, 16777216U)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, 4294967295U)]
    public void ReadFixed32_ReadsNumbers(byte[] data, uint expectedNumber)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var number = reader.ReadFixed32();

            Assert.Equal(expectedNumber, number);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 0UL)]
    [InlineData(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 1UL)]
    [InlineData(new byte[] { 0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, 255UL)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00 }, 4294967295UL)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }, 18446744073709551615UL)]
    public void ReadFixed64_ReadsNumbers(byte[] data, ulong expectedNumber)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var number = reader.ReadFixed64();

            Assert.Equal(expectedNumber, number);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0U)]
    [InlineData(new byte[] { 0x01 }, 1U)]
    [InlineData(new byte[] { 0x7F }, 127U)]
    [InlineData(new byte[] { 0x80, 0x01 }, 128U)]
    [InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384U)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152U)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456U)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, 4294967295U)]
    public void ReadVarint32_ReadsNumbers(byte[] data, uint expectedNumber)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var number = reader.ReadVarInt32();

            Assert.Equal(expectedNumber, number);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, 0UL)]
    [InlineData(new byte[] { 0x01 }, 1UL)]
    [InlineData(new byte[] { 0x7F }, 127UL)]
    [InlineData(new byte[] { 0x80, 0x01 }, 128UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 34359738368UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 4398046511104UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 562949953421312UL)]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 72057594037927936UL)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, 18446744073709551615UL)]
    public void ReadVarint64_ReadsNumbers(byte[] data, ulong expectedNumber)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var number = reader.ReadVarInt64();

            Assert.Equal(expectedNumber, number);
        }
    }
}
