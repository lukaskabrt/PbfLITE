using Xunit;

namespace PbfLite.Tests;

public class PbfBlockWriteReadTests
{
    [Fact]
    public void WriteAndRead_RoundtripWorksCorrectly()
    {
        var buffer = new byte[1024];
        var writeBlock = PbfBlock.Create(buffer);

        // Write various values
        writeBlock.WriteString("Hello World");
        writeBlock.WriteBoolean(true);
        writeBlock.WriteSignedInt(-42);
        writeBlock.WriteInt(12345);
        writeBlock.WriteUint(67890u);
        writeBlock.WriteSignedLong(-9876543210L);
        writeBlock.WriteLong(1234567890123456789L);
        writeBlock.WriteULong(18446744073709551614ul);
        writeBlock.WriteSingle(3.14f);
        writeBlock.WriteDouble(2.718281828);

        // Create a read block from the written data
        var readBlock = PbfBlock.Create(writeBlock.Block);

        // Read and verify all values
        Assert.Equal("Hello World", readBlock.ReadString());
        Assert.True(readBlock.ReadBoolean());
        Assert.Equal(-42, readBlock.ReadSignedInt());
        Assert.Equal(12345, readBlock.ReadInt());
        Assert.Equal(67890u, readBlock.ReadUint());
        Assert.Equal(-9876543210L, readBlock.ReadSignedLong());
        Assert.Equal(1234567890123456789L, readBlock.ReadLong());
        Assert.Equal(18446744073709551614ul, readBlock.ReadULong());
        Assert.Equal(3.14f, readBlock.ReadSingle());
        Assert.Equal(2.718281828, readBlock.ReadDouble());
    }
}