using BenchmarkDotNet.Attributes;

namespace PbfLite.Benchmark;

public class PbfBlockReaderCollectionsBenchmarks
{
    private static readonly byte[] UIntCollectionData = new byte[] { 0x0F, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01 };
    private static readonly uint[] UIntBuffer = new uint[5];

    [Benchmark]
    public uint ReadUIntCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(UIntCollectionData);

        reader.ReadUIntCollection(WireType.String, UIntBuffer);

        return UIntBuffer[0];
    }

    private static readonly byte[] UIntSingleItemCollection = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F };

    [Benchmark]
    public uint ReadUIntSingleItemCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(UIntSingleItemCollection);

        reader.ReadUIntCollection(WireType.VarInt, UIntBuffer);

        return UIntBuffer[0];
    }
}