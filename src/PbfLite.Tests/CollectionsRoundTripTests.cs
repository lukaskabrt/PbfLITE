using System;
using Xunit;

namespace PbfLite.Tests;

public class CollectionsRoundTripTests
{
    [Fact]
    public void UIntCollection_RoundTrip_SingleElement()
    {
        var originalData = new uint[] { 42 };
        var buffer = new byte[10];

        // Write
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteUIntCollection(originalData.AsSpan());

        // Read
        var reader = PbfBlockReader.Create(writer.Block);
        var readBuffer = new uint[1];
        var result = reader.ReadUIntCollection(WireType.String, readBuffer);

        SpanAssert.Equal<uint>(originalData, result);
    }

    [Fact]
    public void UIntCollection_RoundTrip_MultipleElements()
    {
        var originalData = new uint[] { 0, 128, 16384, 2097152 };
        var buffer = new byte[50];

        // Write
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteUIntCollection(originalData.AsSpan());

        // Read
        var reader = PbfBlockReader.Create(writer.Block);
        var readBuffer = new uint[originalData.Length];
        var result = reader.ReadUIntCollection(WireType.String, readBuffer);

        SpanAssert.Equal<uint>(originalData, result);
    }
}