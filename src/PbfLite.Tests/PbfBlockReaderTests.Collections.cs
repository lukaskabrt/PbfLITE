using System;
using System.Collections.Generic;
using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    public class Collections
    {
        [Theory]
        [InlineData(new byte[] { 0x00 }, new uint[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new uint[] { 1 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]

        public void ReadUIntCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, uint[] expectedItems)
        {
            var buffer = new uint[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadUIntCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<uint>(expectedItems.AsSpan(), items);
        }


        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new uint[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new uint[] { 1 })]
        [InlineData(new byte[] { 0x05, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
        [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 4294967295 })]
        [InlineData(new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 })]
        public void ReadUIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, uint[] expectedItems)
        {
            var buffer = new uint[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadUIntCollection(WireType.String, buffer);
            
            SpanAssert.Equal<uint>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new ulong[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new ulong[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
        public void ReadULongCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, ulong[] expectedItems)
        {
            var buffer = new ulong[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadULongCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<ulong>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new ulong[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new ulong[] { 1 })]
        [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 18446744073709551615UL })]
        [InlineData(new byte[] { 0x0C, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new ulong[] { 0, 1, 18446744073709551615UL })]
        public void ReadULongCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, ulong[] expectedItems)
        {
            var buffer = new ulong[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadULongCollection(WireType.String, buffer);
            
            SpanAssert.Equal<ulong>(expectedItems.AsSpan(), items);
        }


        [Theory]
        [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new int[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -2147483648 })]
        public void ReadIntCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, int[] expectedItems)
        {
            var buffer = new int[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadIntCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new int[] { 1 })]
        [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x07 }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0x0A, 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { -2147483648 })]
        [InlineData(new byte[] { 0x19, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x80, 0x80, 0x80, 0xF8, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new int[] { 0, 1, -1, 2147483647, -2147483648 })]
        public void ReadIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, int[] expectedItems)
        {
            var buffer = new int[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadIntCollection(WireType.String, buffer);
            
            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0x02 }, new int[] { 1 })]
        [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -2147483648 })]
        public void ReadSignedIntCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, int[] expectedItems)
        {
            var buffer = new int[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadSignedIntCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new int[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new int[] { -1 })]
        [InlineData(new byte[] { 0x01, 0x02 }, new int[] { 1 })]
        [InlineData(new byte[] { 0x05, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 2147483647 })]
        [InlineData(new byte[] { 0x05, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { -2147483648 })]
        [InlineData(new byte[] { 0x0D, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F }, new int[] { 0, -1, 1, 2147483647, -2147483648 })]
        public void ReadSignedIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, int[] expectedItems)
        {
            var buffer = new int[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadSignedIntCollection(WireType.String, buffer);
            
            SpanAssert.Equal<int>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new long[] { 1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, new long[] { 9223372036854775807 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { -9223372036854775808 })]
        public void ReadLongCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, long[] expectedItems)
        {
            var buffer = new long[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadLongCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new long[] { 1 })]
        [InlineData(new byte[] { 0x0A, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0x09, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F }, new long[] { 9223372036854775807 })]
        [InlineData(new byte[] { 0x0A, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { -9223372036854775808 })]
        [InlineData(new byte[] { 0x1F, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 }, new long[] { 0, 1, -1, 9223372036854775807, -9223372036854775808 })]
        public void ReadLongCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, long[] expectedItems)
        {
            var buffer = new long[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadLongCollection(WireType.String, buffer);
            
            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0x02 }, new long[] { 1 })]
        [InlineData(new byte[] { 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 9223372036854775807 })]
        [InlineData(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { -9223372036854775808 })]
        public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, long[] expectedItems)
        {
            var buffer = new long[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadSignedLongCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new long[] { 0 })]
        [InlineData(new byte[] { 0x01, 0x01 }, new long[] { -1 })]
        [InlineData(new byte[] { 0x01, 0x02 }, new long[] { 1 })]
        [InlineData(new byte[] { 0x17, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 }, new long[] { 0, -1, 1, 9223372036854775807, -9223372036854775808 })]
        public void ReadSignedLongCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, long[] expectedItems)
        {
            var buffer = new long[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadSignedLongCollection(WireType.String, buffer);
            
            SpanAssert.Equal<long>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00 }, new bool[] { false })]
        [InlineData(new byte[] { 0x01 }, new bool[] { true })]
        public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, bool[] expectedItems)
        {
            var buffer = new bool[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadBooleanCollection(WireType.VarInt, buffer);

            SpanAssert.Equal<bool>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x01, 0x00 }, new bool[] { false })]
        [InlineData(new byte[] { 0x01, 0x01 }, new bool[] { true })]
        [InlineData(new byte[] { 0x02, 0x00, 0x01 }, new bool[] { false, true })]
        public void ReadBooleanCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, bool[] expectedItems)
        {
            var buffer = new bool[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadBooleanCollection(WireType.String, buffer);
            
            SpanAssert.Equal<bool>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
        [InlineData(new byte[] { 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
        [InlineData(new byte[] { 0xF7, 0xCC, 0x12, 0x39 }, new float[] { 0.00014f })]
        [InlineData(new byte[] { 0x66, 0x80, 0x3B, 0x46 }, new float[] { 12000.1f })]
        public void ReadSingleCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, float[] expectedItems)
        {
            var buffer = new float[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadSingleCollection(WireType.Fixed32, buffer);

            SpanAssert.Equal<float>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x04, 0x00, 0x00, 0x00, 0x00 }, new float[] { 0 })]
        [InlineData(new byte[] { 0x04, 0xC3, 0xF5, 0x48, 0x40 }, new float[] { 3.14f })]
        [InlineData(new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40, 0xF7, 0xCC, 0x12, 0x39, 0x66, 0x80, 0x3B, 0x46 }, new float[] { 0, 3.14f, 0.00014f, 12000.1f })]
        public void ReadSingleCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, float[] expectedItems)
        {
            var buffer = new float[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadSingleCollection(WireType.String, buffer);
            
            SpanAssert.Equal<float>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
        [InlineData(new byte[] { 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
        [InlineData(new byte[] { 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F }, new double[] { 0.00014 })]
        [InlineData(new byte[] { 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 12000.1 })]
        public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsSingleElementIntoBuffer(byte[] data, double[] expectedItems)
        {
            var buffer = new double[1];
            var reader = PbfBlockReader.Create(data);

            var items = reader.ReadDoubleCollection(WireType.Fixed64, buffer);

            SpanAssert.Equal<double>(expectedItems.AsSpan(), items);
        }

        [Theory]
        [InlineData(new byte[] { 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, new double[] { 0 })]
        [InlineData(new byte[] { 0x08, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40 }, new double[] { 3.14 })]
        [InlineData(new byte[] { 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40, 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F, 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 }, new double[] { 0, 3.14, 0.00014, 12000.1 })]
        public void ReadDoubleCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, double[] expectedItems)
        {
            var buffer = new double[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadDoubleCollection(WireType.String, buffer);
            
            SpanAssert.Equal<double>(expectedItems.AsSpan(), items);
        }
    }
}