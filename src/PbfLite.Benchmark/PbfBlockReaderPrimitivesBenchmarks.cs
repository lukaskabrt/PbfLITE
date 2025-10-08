using BenchmarkDotNet.Attributes;

namespace PbfLite.Benchmark;

public class PbfBlockReaderPrimitivesBenchmarks
{
    private static readonly byte[] Fixed32Data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF };

    [Benchmark]
    public uint ReadFixed32()
    {
        var reader = PbfBlockReader.Create(Fixed32Data);
        reader.ReadFixed32();
        reader.ReadFixed32();
        reader.ReadFixed32();
        reader.ReadFixed32();
        reader.ReadFixed32();
        return reader.ReadFixed32();
    }

    private static readonly byte[] VarInt64Data = new byte[] { 0x01, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };

    [Benchmark]
    public ulong ReadVarInt64()
    {
        var reader = PbfBlockReader.Create(VarInt64Data);
        reader.ReadVarInt64();
        reader.ReadVarInt64();
        reader.ReadVarInt64();
        reader.ReadVarInt64();
        reader.ReadVarInt64();
        return reader.ReadVarInt64();
    }

    private static readonly byte[] VarInt32Data = new byte[] { 0x01, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F };

    [Benchmark]
    public uint ReadVarInt32()
    {
        var reader = PbfBlockReader.Create(VarInt32Data);
        reader.ReadVarInt32();
        reader.ReadVarInt32();
        reader.ReadVarInt32();
        reader.ReadVarInt32();
        reader.ReadVarInt32();
        return reader.ReadVarInt32();
    }
}