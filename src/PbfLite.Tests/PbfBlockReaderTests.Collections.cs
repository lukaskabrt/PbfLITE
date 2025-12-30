using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    public class CollectionsToBuffer : Collections
    {
        protected override CollectionReaderDelegate<uint> UIntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadUIntCollection(wireType, new uint[itemCount]);
        protected override CollectionReaderDelegate<ulong> ULongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadULongCollection(wireType, new ulong[itemCount]);
        protected override CollectionReaderDelegate<int> IntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadIntCollection(wireType, new int[itemCount]);
        protected override CollectionReaderDelegate<int> SignedIntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadSignedIntCollection(wireType, new int[itemCount]);
        protected override CollectionReaderDelegate<long> LongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadLongCollection(wireType, new long[itemCount]);
        protected override CollectionReaderDelegate<long> SignedLongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadSignedLongCollection(wireType, new long[itemCount]);
        protected override CollectionReaderDelegate<bool> BooleanCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadBooleanCollection(wireType, new bool[itemCount]);
        protected override CollectionReaderDelegate<float> SingleCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadSingleCollection(wireType, new float[itemCount]);
        protected override CollectionReaderDelegate<double> DoubleCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) => reader.ReadDoubleCollection(wireType, new double[itemCount]);
    }

    public class CollectionsToList : Collections
    {
        protected override CollectionReaderDelegate<uint> UIntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<uint>(itemCount);
            reader.ReadUIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<ulong> ULongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<ulong>(itemCount);
            reader.ReadULongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<int> IntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<int>(itemCount);
            reader.ReadIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<int> SignedIntCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<int>(itemCount);
            reader.ReadSignedIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<long> LongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<long>(itemCount);
            reader.ReadLongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<long> SignedLongCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<long>(itemCount);
            reader.ReadSignedLongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<bool> BooleanCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<bool>(itemCount);
            reader.ReadBooleanCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<float> SingleCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<float>(itemCount);
            reader.ReadSingleCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };

        protected override CollectionReaderDelegate<double> DoubleCollectionReader => (ref PbfBlockReader reader, WireType wireType, int itemCount) =>
        {
            var list = new List<double>(itemCount);
            reader.ReadDoubleCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        };
    }

    public abstract class Collections
    {
        protected delegate ReadOnlySpan<T> CollectionReaderDelegate<T>(ref PbfBlockReader reader, WireType wireType, int itemCount);

        protected abstract CollectionReaderDelegate<uint> UIntCollectionReader { get; }
        protected abstract CollectionReaderDelegate<ulong> ULongCollectionReader { get; }
        protected abstract CollectionReaderDelegate<int> IntCollectionReader { get; }
        protected abstract CollectionReaderDelegate<int> SignedIntCollectionReader { get; }
        protected abstract CollectionReaderDelegate<long> LongCollectionReader { get; }
        protected abstract CollectionReaderDelegate<long> SignedLongCollectionReader { get; }
        protected abstract CollectionReaderDelegate<bool> BooleanCollectionReader { get; }
        protected abstract CollectionReaderDelegate<float> SingleCollectionReader { get; }
        protected abstract CollectionReaderDelegate<double> DoubleCollectionReader { get; }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new uint[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new uint[] { 1 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]

        public void ReadUIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, uint[] expectedItems)
        {
            var buffer = new uint[1];
            var reader = PbfBlockReader.Create(data);

            var items = UIntCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<uint>(expectedItems.AsSpan(), items);
        }


        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new uint[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new uint[] { 1 })]
        [InlineData(new byte[] { 0x05, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
        [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]
        [InlineData(new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 })]
        public void ReadUIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, uint[] expectedItems)
        {
            var buffer = new uint[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);

            var items = UIntCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<uint>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new ulong[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new ulong[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
        public void ReadULongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, ulong[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = ULongCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<ulong>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new ulong[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new ulong[] { 1 })]
        [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
        [InlineData(new byte[] { 0x0C, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 0, 1, 18446744073709551615UL })]
        public void ReadULongCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, ulong[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = ULongCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<ulong>(expectedItems.AsSpan(), items);
        }


        [Theory]
        [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new int[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -2147483648 })]
        public void ReadIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = IntCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
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
            var reader = PbfBlockReader.Create(data);

            var items = IntCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0x02 }, new int[] { 1 })]
        [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -2147483648 })]
        public void ReadSignedIntCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, int[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = SignedIntCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
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
            var reader = PbfBlockReader.Create(data);

            var items = SignedIntCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new long[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, new long[] { 9223372036854775807 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { -9223372036854775808 })]
        public void ReadLongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = LongCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
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
            var reader = PbfBlockReader.Create(data);

            var items = LongCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0x02 }, new long[] { 1 })]
        [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 9223372036854775807 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -9223372036854775808 })]
        public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, long[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = SignedLongCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0x01, 0x02 }, new long[] { 1 })]
        [InlineData(new byte[] { 0x17, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 0, -1, 1, 9223372036854775807, -9223372036854775808 })]
        public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, long[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = SignedLongCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new bool[] { false })]
        [InlineData(new byte[] { 0x01 }, new bool[] { true })]
        public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, bool[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = BooleanCollectionReader(ref reader, WireType.VarInt, expectedItems.Length);

            SpanAssert.Equal<bool>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new bool[] { false })]
        [InlineData(new byte[] { 0x01, 0x01 }, new bool[] { true })]
        [InlineData(new byte[] { 0x02, 0x00, 0x01 }, new bool[] { false, true })]
        public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, bool[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = BooleanCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<bool>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
        [InlineData(new byte[] { 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
        [InlineData(new byte[] { 0xF7, 0xCC, 0x12, 0x39 }, new float[] { 0.00014f })]
        [InlineData(new byte[] { 0x66, 0x80, 0x3B, 0x46 }, new float[] { 12000.1f })]
        public void ReadSingleCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, float[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = SingleCollectionReader(ref reader, WireType.Fixed32, expectedItems.Length);

            SpanAssert.Equal<float>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
        [InlineData(new byte[] { 0x04, 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
        [InlineData(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40, 0xF7, 0xCC, 0x12, 0x39, 0x66, 0x80, 0x3B, 0x46 }, new float[] { 0, 3.14f, 0.00014f, 12000.1f })]
        public void ReadSingleCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, float[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = SingleCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<float>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
        [InlineData(new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
        [InlineData(new byte[] { 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F }, new double[] { 0.00014 })]
        [InlineData(new byte[] { 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 12000.1 })]
        public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsSingleElement(byte[] data, double[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = DoubleCollectionReader(ref reader, WireType.Fixed64, expectedItems.Length);

            SpanAssert.Equal<double>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
        [InlineData(new byte[] { 0x08, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
        [InlineData(new byte[] { 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40, 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F, 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 0, 3.14, 0.00014, 12000.1 })]
        public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsLengthPrefixed(byte[] data, double[] expectedItems)
        {
            var reader = PbfBlockReader.Create(data);

            var items = DoubleCollectionReader(ref reader, WireType.String, expectedItems.Length);

            SpanAssert.Equal<double>(expectedItems.AsSpan(), items);
        }
    }
}