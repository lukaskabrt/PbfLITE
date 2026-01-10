using System;
using System.IO;
using Xunit;

namespace PbfLite.Tests;

public partial class PbfStreamReaderTests
{
    public class SystemTypes : PbfReaderSystemTypesTests
    {
        public override string ReadString(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);

            return reader.ReadString();
        }

        public override bool ReadBoolean(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadBoolean();
        }

        public override int ReadSignedInt(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadSignedInt();
        }

        public override int ReadInt(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadInt();
        }

        public override uint ReadUint(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadUint();
        }

        public override long ReadSignedLong(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadSignedLong();
        }

        public override long ReadLong(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadLong();
        }

        public override ulong ReadULong(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadULong();
        }

        public override float ReadSingle(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadSingle();
        }

        public override double ReadDouble(byte[] data)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            
            return reader.ReadDouble();
        }
    }
}