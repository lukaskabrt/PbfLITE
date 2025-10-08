using BenchmarkDotNet.Attributes;

namespace PbfLite.Benchmark;

public class PbfBlockWriterPrimitivesBenchmarks
{
    [Benchmark]
    public int WriteFixed32()
    {
        var buffer = new byte[24];
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteFixed32(1);
        writer.WriteFixed32(255);
        writer.WriteFixed32(256);
        writer.WriteFixed32(65536);
        writer.WriteFixed32(16777216);
        writer.WriteFixed32(4294967295);
        return writer.Position;
    }

    [Benchmark]
    public int WriteVarInt64()
    {
        var buffer = new byte[60];
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteVarInt64(1);
        writer.WriteVarInt64(128);
        writer.WriteVarInt64(16384);
        writer.WriteVarInt64(2097152);
        writer.WriteVarInt64(268435456);
        writer.WriteVarInt64(18446744073709551615UL);
        return writer.Position;
    }

    [Benchmark]
    public int WriteVarInt32()
    {
        var buffer = new byte[30];
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteVarInt32(1);
        writer.WriteVarInt32(128);
        writer.WriteVarInt32(16384);
        writer.WriteVarInt32(2097152);
        writer.WriteVarInt32(268435456);
        writer.WriteVarInt32(4294967295);
        return writer.Position;
    }
}