using System;
using Xunit;

namespace PbfLite.Tests;

public class PbfBlockReaderWriterTests
{
    [Fact]
    public void SeparateReaderWriter_RoundtripWorksCorrectly()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);

        // Write various values
        writer.WriteString("Hello World");
        writer.WriteBoolean(true);
        writer.WriteSignedInt(-42);
        writer.WriteInt(12345);
        writer.WriteUint(67890u);
        writer.WriteSignedLong(-9876543210L);
        writer.WriteLong(1234567890123456789L);
        writer.WriteULong(18446744073709551614ul);
        writer.WriteSingle(3.14f);
        writer.WriteDouble(2.718281828);

        // Create a reader from the written data
        var reader = PbfBlockReader.Create(writer.Block);

        // Read and verify all values
        Assert.Equal("Hello World", reader.ReadString());
        Assert.True(reader.ReadBoolean());
        Assert.Equal(-42, reader.ReadSignedInt());
        Assert.Equal(12345, reader.ReadInt());
        Assert.Equal(67890u, reader.ReadUint());
        Assert.Equal(-9876543210L, reader.ReadSignedLong());
        Assert.Equal(1234567890123456789L, reader.ReadLong());
        Assert.Equal(18446744073709551614ul, reader.ReadULong());
        Assert.Equal(3.14f, reader.ReadSingle());
        Assert.Equal(2.718281828, reader.ReadDouble());
    }

    [Fact]
    public void ReaderOnlyRequiresReadOnlySpan()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteString("Test");
        writer.WriteInt(42);

        // Reader can work with ReadOnlySpan
        ReadOnlySpan<byte> readOnlyData = writer.Block;
        var reader = PbfBlockReader.Create(readOnlyData);

        Assert.Equal("Test", reader.ReadString());
        Assert.Equal(42, reader.ReadInt());
    }
}