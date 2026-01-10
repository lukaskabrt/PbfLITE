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
            var reader = new PbfStreamReader(new MemoryStream(data));
            return reader.ReadFixed32();
        }

        public override ulong ReadFixed64(byte[] data)
        {
            var reader = new PbfStreamReader(new MemoryStream(data));
            return reader.ReadFixed64();
        }

        public override ReadOnlySpan<byte> ReadLengthPrefixedBytes(byte[] data)
        {
            var reader = new PbfStreamReader(new MemoryStream(data));
            return reader.ReadLengthPrefixedBytes();
        }

        public override uint ReadVarInt32(byte[] data)
        {
            var reader = new PbfStreamReader(new MemoryStream(data));
            return reader.ReadVarInt32();
        }

        public override ulong ReadVarInt64(byte[] data)
        {
            var reader = new PbfStreamReader(new MemoryStream(data));
            return reader.ReadVarInt64();
        }
    }
}