using System;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public partial class PbfStreamReaderTests
{
    public class Primitives : PbfReaderPrimitivesTests
    {
        public override uint ReadFixed32(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            return reader.ReadFixed32();
        }

        public override ulong ReadFixed64(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            return reader.ReadFixed64();
        }

        public override ReadOnlySpan<byte> ReadLengthPrefixedBytes(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            return reader.ReadLengthPrefixedBytes();
        }

        public override uint ReadVarInt32(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            return reader.ReadVarInt32();
        }

        public override ulong ReadVarInt64(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            return reader.ReadVarInt64();
        }
    }
}