using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public class PbfStreamReaderTests_Collections
{
    [Theory]
    [InlineData(new byte[] { 0x00 }, new uint[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new uint[] { 1 })]
    [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]
    public void ReadUIntCollection_ReadsDataSerializedAsSingleElement(byte[] data, uint[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<uint>();

            reader.ReadUIntCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new uint[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new uint[] { 1 })]
    [InlineData(new byte[] { 0x05, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
    [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]
    [InlineData(new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 })]
    public void ReadUIntCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, uint[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<uint>();

            reader.ReadUIntCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new ulong[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new ulong[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
    public void ReadULongCollection_ReadsDataSerializedAsSingleElement(byte[] data, ulong[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<ulong>();

            reader.ReadULongCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new ulong[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new ulong[] { 1 })]
    public void ReadULongCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, ulong[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<ulong>();

            reader.ReadULongCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new int[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -1 })]
    public void ReadIntCollection_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<int>();

            reader.ReadIntCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new int[] { 1 })]
    [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -1 })]
    public void ReadIntCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, int[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<int>();

            reader.ReadIntCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new long[] { 1 })]
    [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
    public void ReadLongCollection_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<long>();

            reader.ReadLongCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new long[] { 1 })]
    public void ReadLongCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, long[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<long>();

            reader.ReadLongCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0x02 }, new int[] { 1 })]
    public void ReadSignedIntCollection_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<int>();

            reader.ReadSignedIntCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new int[] { -1 })]
    [InlineData(new byte[] { 0x01, 0x02 }, new int[] { 1 })]
    public void ReadSignedIntCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, int[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<int>();

            reader.ReadSignedIntCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0x02 }, new long[] { 1 })]
    public void ReadSignedLongCollection_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<long>();

            reader.ReadSignedLongCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
    [InlineData(new byte[] { 0x01, 0x01 }, new long[] { -1 })]
    [InlineData(new byte[] { 0x01, 0x02 }, new long[] { 1 })]
    public void ReadSignedLongCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, long[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<long>();

            reader.ReadSignedLongCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00 }, new bool[] { false })]
    [InlineData(new byte[] { 0x01 }, new bool[] { true })]
    public void ReadBooleanCollection_ReadsDataSerializedAsSingleElement(byte[] data, bool[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<bool>();

            reader.ReadBooleanCollection(WireType.VarInt, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x02, 0x00, 0x01 }, new bool[] { false, true })]
    [InlineData(new byte[] { 0x04, 0x01, 0x00, 0x01, 0x00 }, new bool[] { true, false, true, false })]
    public void ReadBooleanCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, bool[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<bool>();

            reader.ReadBooleanCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, new float[] { 0f })]
    [InlineData(new byte[] { 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
    public void ReadSingleCollection_ReadsDataSerializedAsSingleElement(byte[] data, float[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<float>();

            reader.ReadSingleCollection(WireType.Fixed32, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00 }, new float[] { 0f })]
    [InlineData(new byte[] { 0x08, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 0f, 3.14f })]
    public void ReadSingleCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, float[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<float>();

            reader.ReadSingleCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0d })]
    [InlineData(new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
    public void ReadDoubleCollection_ReadsDataSerializedAsSingleElement(byte[] data, double[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<double>();

            reader.ReadDoubleCollection(WireType.Fixed64, items);

            Assert.Equal(expectedItems, items);
        }
    }

    [Theory]
    [InlineData(new byte[] { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0d })]
    [InlineData(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 0d, 3.14 })]
    public void ReadDoubleCollection_ReadsDataSerializedAsLengthPrefixed(byte[] data, double[] expectedItems)
    {
        using (var stream = new MemoryStream(data))
        {
            var reader = new PbfStreamReader(stream);
            var items = new List<double>();

            reader.ReadDoubleCollection(WireType.String, items);

            Assert.Equal(expectedItems, items);
        }
    }
}
