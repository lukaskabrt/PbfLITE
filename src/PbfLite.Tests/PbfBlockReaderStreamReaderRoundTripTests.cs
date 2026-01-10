using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public class PbfBlockReaderStreamReaderRoundTripTests
{
    [Fact]
    public void BlockWriterAndStreamReader_RoundtripWorksCorrectly()
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

        // Create a stream reader from the written data
        using (var stream = new MemoryStream(writer.Block.ToArray()))
        {
            var reader = new PbfStreamReader(stream);

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
    }

    [Fact]
    public void BlockReaderAndBlockWriter_ProduceIdenticalResults()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);

        // Write test data
        writer.WriteSignedInt(-12345);
        writer.WriteString("Test String");
        writer.WriteBoolean(false);

        // Read with BlockReader
        var blockReader = PbfBlockReader.Create(writer.Block);
        var blockSignedInt = blockReader.ReadSignedInt();
        var blockString = blockReader.ReadString();
        var blockBool = blockReader.ReadBoolean();

        // Read with StreamReader
        using (var stream = new MemoryStream(writer.Block.ToArray()))
        {
            var streamReader = new PbfStreamReader(stream);
            var streamSignedInt = streamReader.ReadSignedInt();
            var streamString = streamReader.ReadString();
            var streamBool = streamReader.ReadBoolean();

            // Both readers should produce identical results
            Assert.Equal(blockSignedInt, streamSignedInt);
            Assert.Equal(blockString, streamString);
            Assert.Equal(blockBool, streamBool);
        }
    }

    [Fact]
    public void StreamReader_HandlesFieldHeaders_SameAsBlockReader()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);

        // Write field headers and values
        writer.WriteFieldHeader(1, WireType.String);  // Field 1, string wire type
        writer.WriteString("Value1");
        writer.WriteFieldHeader(2, WireType.VarInt);  // Field 2, varint wire type
        writer.WriteInt(42);

        // Read with BlockReader
        var blockReader = PbfBlockReader.Create(writer.Block);
        var header1 = blockReader.ReadFieldHeader();
        var val1 = blockReader.ReadString();
        var header2 = blockReader.ReadFieldHeader();
        var val2 = blockReader.ReadInt();

        // Read with StreamReader
        using (var stream = new MemoryStream(writer.Block.ToArray()))
        {
            var streamReader = new PbfStreamReader(stream);
            var sHeader1 = streamReader.ReadFieldHeader();
            var sVal1 = streamReader.ReadString();
            var sHeader2 = streamReader.ReadFieldHeader();
            var sVal2 = streamReader.ReadInt();

            // Verify field headers match
            Assert.Equal(header1.fieldNumber, sHeader1.fieldNumber);
            Assert.Equal(header1.wireType, sHeader1.wireType);
            Assert.Equal(header2.fieldNumber, sHeader2.fieldNumber);
            Assert.Equal(header2.wireType, sHeader2.wireType);

            // Verify values match
            Assert.Equal(val1, sVal1);
            Assert.Equal(val2, sVal2);
        }
    }

    [Fact]
    public void StreamReader_WithCollections_MatchesBlockReader()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);

        // Write a collection
        uint[] testData = { 10, 20, 30, 40, 50 };
        writer.WriteUIntCollection(testData);

        // Read with BlockReader
        var blockReader = PbfBlockReader.Create(writer.Block);
        var blockItems = blockReader.ReadUIntCollection(WireType.String, new uint[testData.Length]);

        // Read with StreamReader
        using (var stream = new MemoryStream(writer.Block.ToArray()))
        {
            var streamReader = new PbfStreamReader(stream);
            var streamItems = new List<uint>();
            streamReader.ReadUIntCollection(WireType.String, streamItems);

            // Verify both readers get the same collection data
            Assert.Equal(blockItems.Length, streamItems.Count);
            for (int i = 0; i < testData.Length; i++)
            {
                Assert.Equal(blockItems[i], streamItems[i]);
            }
        }
    }

    [Fact]
    public void StreamReader_ReadsMinimalBytesPerPrimitive()
    {
        // Create a buffer with data for a varint 5, followed by junk
        byte[] data = { 0x05, 0xFF, 0xFF, 0xFF };
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadVarInt32();
            
            // Should have read exactly 1 byte (just the varint 5)
            Assert.Equal(5u, value);
            Assert.Equal(1, stream.Position);
        }
    }

    [Fact]
    public void StreamReader_ReadsExactFixedSize()
    {
        byte[] data = { 0x01, 0x02, 0x03, 0x04, 0xFF, 0xFF, 0xFF, 0xFF };
        
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            
            var value = reader.ReadFixed32();
            
            // Should have read exactly 4 bytes
            Assert.Equal(0x04030201U, value);
            Assert.Equal(4, stream.Position);
        }
    }

    [Fact]
    public void StreamReader_SkipsFieldsCorrectly()
    {
        var buffer = new byte[1024];
        var writer = PbfBlockWriter.Create(buffer);

        writer.WriteFieldHeader(1, WireType.VarInt);
        writer.WriteInt(12345);
        writer.WriteFieldHeader(2, WireType.String);
        writer.WriteString("Skip me");
        writer.WriteFieldHeader(3, WireType.VarInt);
        writer.WriteInt(67890);

        using (var stream = new MemoryStream(writer.Block.ToArray()))
        {
            var reader = new PbfStreamReader(stream);

            var header1 = reader.ReadFieldHeader();
            var value1 = reader.ReadInt();

            var header2 = reader.ReadFieldHeader();
            reader.SkipField(header2.wireType);  // Skip field 2

            var header3 = reader.ReadFieldHeader();
            var value3 = reader.ReadInt();

            Assert.Equal(1, header1.fieldNumber);
            Assert.Equal(12345, value1);
            Assert.Equal(2, header2.fieldNumber);
            Assert.Equal(3, header3.fieldNumber);
            Assert.Equal(67890, value3);
        }
    }
}
