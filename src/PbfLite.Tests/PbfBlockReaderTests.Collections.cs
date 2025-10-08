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
        [InlineData(new byte[] { 0x80, 0x01 }, new uint[] { 128 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x01 }, new uint[] { 16384 })]
        [InlineData(new byte[] { 0x80, 0x80, 0x80, 0x01 }, new uint[] { 2097152 })]
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
        [InlineData(new byte[] { 0x02, 0x80, 0x01 }, new uint[] { 128 })]
        [InlineData(new byte[] { 0x03, 0x80, 0x80, 0x01 }, new uint[] { 16384 })]
        [InlineData(new byte[] { 0x04, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 2097152 })]
        [InlineData(new byte[] { 0x05, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 268435456 })]
        [InlineData(new byte[] { 0x0F, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01 }, new uint[] { 0, 128, 16384, 2097152, 268435456 })]
        public void ReadUIntCollection_Buffer_ReadsDataSerializedAsLengthPrefixedIntoBuffer(byte[] data, uint[] expectedItems)
        {
            var buffer = new uint[expectedItems.Length];
            var reader = PbfBlockReader.Create(data);
         
            var items = reader.ReadUIntCollection(WireType.String, buffer);
            
            SpanAssert.Equal<uint>(expectedItems.AsSpan(), items);
        }
    }
}