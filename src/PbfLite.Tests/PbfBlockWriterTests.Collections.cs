using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockWriterTests
{
    public class Collections
    {
        [Fact]
        public void WriteUIntCollection_WritesLongCollection()
        {
            var items = Enumerable.Repeat((uint)1, 1000).ToArray();

            var encodedLength = new byte[] { 0xE8, 0x07 };
            var expectedContent = Enumerable.Repeat((byte)1, items.Length).ToArray();
            var expectedBytes = encodedLength.Concat(expectedContent).ToArray();

            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);
            writer.WriteUIntCollection(items.AsSpan());
            
            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 }, new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void WriteUIntCollection_MultipleElements_WritesDataWithLengthPrefix(uint[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteUIntCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new uint[] { 0, 128, 16384, 2097152, 268435456, 4294967295 }, new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void WriteUIntCollection_WithIEnumerable_WritesDataWithLengthPrefix(uint[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteUIntCollection((IEnumerable<uint>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new ulong[] { 0, 1, 18446744073709551615UL }, new byte[] { 0x0C, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteULongCollection_MultipleElements_WritesDataWithLengthPrefix(ulong[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteULongCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new ulong[] { 0, 1, 18446744073709551615UL }, new byte[] { 0x0C, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteULongCollection_WithIEnumerable_WritesDataWithLengthPrefix(ulong[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteULongCollection((IEnumerable<ulong>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new int[] { 0, 1, -1, 2147483647, -2147483648 }, new byte[] { 0x11, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x80, 0x80, 0x80, 0x08 })]
        public void WriteIntCollection_MultipleElements_WritesDataWithLengthPrefix(int[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteIntCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new int[] { 0, 1, -1, 2147483647, -2147483648 }, new byte[] { 0x11, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x07, 0x80, 0x80, 0x80, 0x80, 0x08 })]
        public void WriteIntCollection_WithIEnumerable_WritesDataWithLengthPrefix(int[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteIntCollection((IEnumerable<int>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new int[] { 0, -1, 1, 2147483647, -2147483648 }, new byte[] { 0x0D, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void WriteSignedIntCollection_MultipleElements_WritesDataWithLengthPrefix(int[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSignedIntCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new int[] { 0, -1, 1, 2147483647, -2147483648 }, new byte[] { 0x0D, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0x0F, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F })]
        public void WriteSignedIntCollection_WithIEnumerable_WritesDataWithLengthPrefix(int[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSignedIntCollection((IEnumerable<int>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new long[] { 0, 1, -1, 9223372036854775807, -9223372036854775808 }, new byte[] { 0x1F, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        public void WriteLongCollection_MultipleElements_WritesDataWithLengthPrefix(long[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteLongCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new long[] { 0, 1, -1, 9223372036854775807, -9223372036854775808 }, new byte[] { 0x1F, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x80, 0x01 })]
        public void WriteLongCollection_WithIEnumerable_WritesDataWithLengthPrefix(long[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteLongCollection((IEnumerable<long>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new long[] { 0, -1, 1, 9223372036854775807, -9223372036854775808 }, new byte[] { 0x17, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteSignedLongCollection_MultipleElements_WritesDataWithLengthPrefix(long[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSignedLongCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new long[] { 0, -1, 1, 9223372036854775807, -9223372036854775808 }, new byte[] { 0x17, 0x00, 0x01, 0x02, 0xFE, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 })]
        public void WriteSignedLongCollection_WithIEnumerable_WritesDataWithLengthPrefix(long[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSignedLongCollection((IEnumerable<long>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new bool[] { false, true }, new byte[] { 0x02, 0x00, 0x01 })]
        public void WriteBooleanCollection_MultipleElements_WritesDataWithLengthPrefix(bool[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteBooleanCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new bool[] { false, true }, new byte[] { 0x02, 0x00, 0x01 })]
        public void WriteBooleanCollection_WithIEnumerable_WritesDataWithLengthPrefix(bool[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteBooleanCollection((IEnumerable<bool>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new float[] { 0, 3.14f, 0.00014f, 12000.1f }, new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40, 0xF7, 0xCC, 0x12, 0x39, 0x66, 0x80, 0x3B, 0x46 })]
        public void WriteSingleCollection_MultipleElements_WritesDataWithLengthPrefix(float[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSingleCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new float[] { 0, 3.14f, 0.00014f, 12000.1f }, new byte[] { 0x10, 0x00, 0x00, 0x00, 0x00, 0xC3, 0xF5, 0x48, 0x40, 0xF7, 0xCC, 0x12, 0x39, 0x66, 0x80, 0x3B, 0x46 })]
        public void WriteSingleCollection_WithIEnumerable_WritesDataWithLengthPrefix(float[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteSingleCollection((IEnumerable<float>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new double[] { 0, 3.14, 0.00014, 12000.1 }, new byte[] { 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40, 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F, 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 })]
        public void WriteDoubleCollection_MultipleElements_WritesDataWithLengthPrefix(double[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteDoubleCollection(items.AsSpan());

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }

        [Theory]
        [InlineData(new double[] { 0, 3.14, 0.00014, 12000.1 }, new byte[] { 0x20, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x1F, 0x85, 0xEB, 0x51, 0xB8, 0x1E, 0x09, 0x40, 0xD2, 0xFB, 0xC6, 0xD7, 0x9E, 0x59, 0x22, 0x3F, 0xCD, 0xCC, 0xCC, 0xCC, 0x0C, 0x70, 0xC7, 0x40 })]
        public void WriteDoubleCollection_WithIEnumerable_WritesDataWithLengthPrefix(double[] items, byte[] expectedBytes)
        {
            var writer = PbfBlockWriter.Create(new byte[expectedBytes.Length]);

            writer.WriteDoubleCollection((IEnumerable<double>)items);

            SpanAssert.Equal<byte>(expectedBytes, writer.Block);
        }
    }
}