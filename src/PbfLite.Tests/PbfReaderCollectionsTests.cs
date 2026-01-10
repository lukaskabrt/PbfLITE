using System;
using Xunit;

namespace PbfLite.Tests;

public abstract class PbfReaderCollectionsTests
{
    protected abstract ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount);
    protected abstract ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount);

    [Theory]
    [InlineData(new byte[] { 0x00 }, new uint[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new uint[] { 1 })]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]

    public void ReadUIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, uint[] expectedItems)
    {
        var items = ReadUIntCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }


    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new uint[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new uint[] { 1 })]
    [InlineData(new byte[] { 0x05, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
    [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]
    [InlineData(new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 })]
    public void ReadUIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, uint[] expectedItems)
    {
        var items = ReadUIntCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new ulong[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new ulong[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
    public void ReadULongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, ulong[] expectedItems)
    {
        var items = ReadULongCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new ulong[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new ulong[] { 1 })]
    [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
    [InlineData(new byte[] { 0x0C, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 0, 1, 18446744073709551615UL })]
    public void ReadULongCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, ulong[] expectedItems)
    {
        var items = ReadULongCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new int[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, new int[] { 2147483647 })]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -2147483648 })]
    public void ReadIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
    {
        var items = ReadIntCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new int[] { 1 })]
    [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, new int[] { 2147483647 })]
    [InlineData(new byte[] { 0x0A, 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -2147483648 })]
    [InlineData(new byte[] { 0x19, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { 0, 1, -1, 2147483647, -2147483648 })]
    public void ReadIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, int[] expectedItems)
    {
        var items = ReadIntCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0x02 }, new int[] { 1 })]
    [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 2147483647 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -2147483648 })]
    public void ReadSignedIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
    {
        var items = ReadSignedIntCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0x01, 0x02 }, new int[] { 1 })]
    [InlineData(new byte[] { 0x05, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 2147483647 })]
    [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -2147483648 })]
    [InlineData(new byte[] { 0x0D, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 0, -1, 1, 2147483647, -2147483648 })]
    public void ReadSignedIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, int[] expectedItems)
    {
        var items = ReadSignedIntCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new long[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, new long[] { 9223372036854775807 })]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { -9223372036854775808 })]
    public void ReadLongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
    {
        var items = ReadLongCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new long[] { 1 })]
    [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, new long[] { 9223372036854775807 })]
    [InlineData(new byte[] { 0x0A, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { -9223372036854775808 })]
    [InlineData(new byte[] { 0x1F, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { 0, 1, -1, 9223372036854775807, -9223372036854775808 })]
    public void ReadLongCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, long[] expectedItems)
    {
        var items = ReadLongCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0x02 }, new long[] { 1 })]
    [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 9223372036854775807 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -9223372036854775808 })]
    public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
    {
        var items = ReadSignedLongCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0x01, 0x02 }, new long[] { 1 })]
    [InlineData(new byte[] { 0x17, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 0, -1, 1, 9223372036854775807, -9223372036854775808 })]
    public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, long[] expectedItems)
    {
        var items = ReadSignedLongCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new bool[] { false })]
    [InlineData(new byte[] { 0x01 }, new bool[] { true })]
    public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, bool[] expectedItems)
    {
        var items = ReadBooleanCollection(data, WireType.VarInt, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new bool[] { false })]
    [InlineData(new byte[] { 0x01, 0x01 }, new bool[] { true })]
    [InlineData(new byte[] { 0x02, 0x00, 0x01 }, new bool[] { false, true })]
    public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, bool[] expectedItems)
    {
        var items = ReadBooleanCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
    [InlineData(new byte[] { 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
    [InlineData(new byte[] { 0xF7, 0xCC, 0x12, 0x39 }, new float[] { 0.00014f })]
    [InlineData(new byte[] { 0x66, 0x80, 0x3B, 0x46 }, new float[] { 12000.1f })]
    public void ReadSingleCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, float[] expectedItems)
    {
        var items = ReadSingleCollection(data, WireType.Fixed32, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
    [InlineData(new byte[] { 0x04, 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
    [InlineData(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40, 0xF7, 0xCC, 0x12, 0x39, 0x66, 0x80, 0x3B, 0x46 }, new float[] { 0, 3.14f, 0.00014f, 12000.1f })]
    public void ReadSingleCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, float[] expectedItems)
    {
        var items = ReadSingleCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
    [InlineData(new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
    [InlineData(new byte[] { 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F }, new double[] { 0.00014 })]
    [InlineData(new byte[] { 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 12000.1 })]
    public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, double[] expectedItems)
    {
        var items = ReadDoubleCollection(data, WireType.Fixed64, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }

    [Theory]
    [InlineData(new byte[] { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
    [InlineData(new byte[] { 0x08, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
    [InlineData(new byte[] { 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40, 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F, 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 0, 3.14, 0.00014, 12000.1 })]
    public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, double[] expectedItems)
    {
        var items = ReadDoubleCollection(data, WireType.String, expectedItems.Length);

        SpanAssert.Equal(expectedItems.AsSpan(), items);
    }
}
