using System;
using Xunit;

namespace PbfLite.Tests;

public class PbfBlockPrimitivesWritesTests
{
    [Theory]
    [InlineData(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
    [InlineData(1, new byte[] { 0x01, 0x00, 0x00, 0x00 })]
    [InlineData(255, new byte[] { 0xFF, 0x00, 0x00, 0x00 })]
    [InlineData(256, new byte[] { 0x00, 0x01, 0x00, 0x00 })]
    [InlineData(65536, new byte[] { 0x00, 0x00, 0x01, 0x00 })]
    [InlineData(16777216, new byte[] { 0x00, 0x00, 0x00, 0x01 })]
    [InlineData(4294967295, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]
    public void ReadFixed32_WritesNumbers(uint number, byte[] expectedData)
    {
        var buffer = new byte[4];
        var block = PbfBlock.Create(buffer);

        block.WriteFixed32(number);

        Assert.Collection(buffer,
            b0 => Assert.Equal(expectedData[0], b0),
            b1 => Assert.Equal(expectedData[1], b1),
            b2 => Assert.Equal(expectedData[2], b2),
            b3 => Assert.Equal(expectedData[3], b3)
        );
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
        var block = PbfBlock.Create(buffer);

        block.WriteFixed64(number);

        Assert.Collection(buffer,
            b0 => Assert.Equal(expectedData[0], b0),
            b1 => Assert.Equal(expectedData[1], b1),
            b2 => Assert.Equal(expectedData[2], b2),
            b3 => Assert.Equal(expectedData[3], b3),
            b4 => Assert.Equal(expectedData[4], b4),
            b5 => Assert.Equal(expectedData[5], b5),
            b6 => Assert.Equal(expectedData[6], b6),
            b7 => Assert.Equal(expectedData[7], b7)
        );
    }

    //[Fact]
    //public void ReadLengthPrefixedBytes_ReadsData()
    //{
    //    var block = PbfBlock.Create([0x03, 0x41, 0x42, 0x43]);

    //    var data = block.ReadLengthPrefixedBytes();

    //    Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, data.ToArray());
    //}

    //[Theory]
    //[InlineData(new byte[] { 0x00 }, 0)]
    //[InlineData(new byte[] { 0x01 }, 1)]
    //[InlineData(new byte[] { 0x7F }, 127)]
    //[InlineData(new byte[] { 0x80, 0x01 }, 128)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456)]
    //[InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, 4294967295)]
    //public void ReadVarint32_ReadsNumbers(byte[] data, uint expectedNumber)
    //{
    //    var block = PbfBlock.Create(data);

    //    var number = block.ReadVarint32();

    //    Assert.Equal(expectedNumber, number);
    //}

    //[Theory]
    //[InlineData(new byte[] { 0x00 }, 0)]
    //[InlineData(new byte[] { 0x01 }, 1)]
    //[InlineData(new byte[] { 0x7F }, 127)]
    //[InlineData(new byte[] { 0x80, 0x01 }, 128)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x01 }, 16384)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, 2097152)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, 268435456)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 34359738368UL)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 4398046511104UL)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 562949953421312UL)]
    //[InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, 72057594037927936UL)]
    //[InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, 18446744073709551615UL)]
    //public void ReadVarint64_ReadsNumbers(byte[] data, ulong expectedNumber)
    //{
    //    var block = PbfBlock.Create(data);

    //    var number = block.ReadVarint64();

    //    Assert.Equal(expectedNumber, number);
    //}
}
