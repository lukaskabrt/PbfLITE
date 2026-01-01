using System;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public class PbfStreamReaderTests
{
    [Theory]
    [InlineData(new byte[] { 0x08 }, 1)]
    [InlineData(new byte[] { 0x78 }, 15)]
    [InlineData(new byte[] { 0x80, 0x01 }, 16)]
    public void ReadFieldHeader_ReadsFieldNumbers(byte[] data, int fieldNumber)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var header = reader.ReadFieldHeader();

            Assert.Equal(fieldNumber, header.fieldNumber);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x08 }, WireType.VarInt)]
    [InlineData(new byte[] { 0x09 }, WireType.Fixed64)]
    [InlineData(new byte[] { 0x0A }, WireType.String)]
    [InlineData(new byte[] { 0x0D }, WireType.Fixed32)]
    public void ReadFieldHeader_ReadsWireTypes(byte[] data, WireType wireType)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            var header = reader.ReadFieldHeader();

            Assert.Equal(wireType, header.wireType);
        }
    }

    [Fact]
    public void ReadFieldHeader_ReturnsNoneWhenEndOfStreamIsReached()
    {
        using (var stream = new MemoryStream(Array.Empty<byte>()))
        {
            var reader = new PbfStreamReader(stream);

            var header = reader.ReadFieldHeader();

            Assert.Equal(0, header.fieldNumber);
            Assert.Equal(WireType.None, header.wireType);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, WireType.VarInt, 1)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, WireType.VarInt, 5)]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, WireType.VarInt, 10)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed32, 4)]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, WireType.Fixed64, 8)]
    [InlineData(new byte[] { 0x03, 0x41, 0x42, 0x43 }, WireType.String, 4)]
    public void SkipFields_SkipsCorrectNumberOfBytes(byte[] data, WireType wireType, int expectedPosition)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);

            reader.SkipField(wireType);

            Assert.Equal(expectedPosition, stream.Position);
        }
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(4294967294U, 2147483647)]
    [InlineData(4294967295U, -2147483648)]
    public void Zag_Decodes32BitZiggedValues(uint encodedNumber, int expectedNumber)
    {
        var number = PbfEncodingHelpers.Zag(encodedNumber);

        Assert.Equal(expectedNumber, number);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, -1)]
    [InlineData(2, 1)]
    [InlineData(18446744073709551614UL, 9223372036854775807L)]
    [InlineData(18446744073709551615UL, -9223372036854775808L)]
    public void Zag_Decodes64BitZiggedValues(ulong encodedNumber, long expectedNumber)
    {
        var number = PbfEncodingHelpers.Zag(encodedNumber);

        Assert.Equal(expectedNumber, number);
    }

    [Fact]
    public void ReadVarInt32_ReadsMinimalBytes()
    {
        // Test that ReadVarInt32 reads exactly as many bytes as needed
        // 0x05 = varint for value 5 (single byte)
        byte[] data = { 0x05, 0xFF };  // 0xFF should not be read
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadVarInt32();
            
            Assert.Equal(5U, value);
            Assert.Equal(1, stream.Position);  // Only one byte should have been read
        }
    }

    [Fact]
    public void ReadVarInt64_ReadsMinimalBytes()
    {
        // Test that ReadVarInt64 reads exactly as many bytes as needed
        // 0x7F = varint for value 127 (single byte)
        byte[] data = { 0x7F, 0xFF };  // 0xFF should not be read
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadVarInt64();
            
            Assert.Equal(127UL, value);
            Assert.Equal(1, stream.Position);  // Only one byte should have been read
        }
    }

    [Fact]
    public void ReadFixed32_ReadsExactly4Bytes()
    {
        byte[] data = { 0x01, 0x02, 0x03, 0x04, 0xFF };
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadFixed32();
            
            Assert.Equal(0x04030201U, value);  // Little-endian
            Assert.Equal(4, stream.Position);  // Exactly 4 bytes read
        }
    }

    [Fact]
    public void ReadFixed64_ReadsExactly8Bytes()
    {
        byte[] data = { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0xFF };
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadFixed64();
            
            Assert.Equal(0x0807060504030201UL, value);  // Little-endian
            Assert.Equal(8, stream.Position);  // Exactly 8 bytes read
        }
    }

    [Fact]
    public void ReadLengthPrefixedBytes_ReadsExactLength()
    {
        // Length = 3, followed by "ABC"
        byte[] data = { 0x03, 0x41, 0x42, 0x43, 0xFF };
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var bytes = reader.ReadLengthPrefixedBytes();
            
            Assert.Equal(new byte[] { 0x41, 0x42, 0x43 }, bytes);
            Assert.Equal(4, stream.Position);  // 1 (length varint) + 3 (data) bytes read
        }
    }

    [Fact]
    public void Dispose_ClosesStream()
    {
        var stream = new MemoryStream(new byte[] { 0x00 });
        var reader = new PbfStreamReader(stream);
        
        reader.Dispose();
        
        // Stream should be closed
        Assert.Throws<ObjectDisposedException>(() => stream.WriteByte(0));
    }

    [Fact]
    public void ThrowsAfterDispose()
    {
        var stream = new MemoryStream(new byte[] { 0x00 });
        var reader = new PbfStreamReader(stream);
        
        reader.Dispose();
        
        // Operations should throw ObjectDisposedException
        Assert.Throws<ObjectDisposedException>(() => reader.ReadFieldHeader());
    }
}
