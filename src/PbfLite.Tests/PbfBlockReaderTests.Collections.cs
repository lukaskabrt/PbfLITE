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
    }
}