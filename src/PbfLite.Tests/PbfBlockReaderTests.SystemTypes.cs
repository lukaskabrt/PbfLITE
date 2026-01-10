using Xunit;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    public class SystemTypes : PbfReaderSystemTypesTests
    {
        public override string ReadString(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadString();
        }

        public override bool ReadBoolean(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadBoolean();
        }

        public override int ReadSignedInt(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadSignedInt();
        }

        public override int ReadInt(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadInt();
        }

        public override uint ReadUint(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadUint();
        }

        public override long ReadSignedLong(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadSignedLong();
        }

        public override long ReadLong(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadLong();
        }

        public override ulong ReadULong(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadULong();
        }

        public override float ReadSingle(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadSingle();
        }

        public override double ReadDouble(byte[] data)
        {
            var reader = PbfBlockReader.Create(data);
            return reader.ReadDouble();
        }
    }
}