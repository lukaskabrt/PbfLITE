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

    private static readonly byte[] ULongCollectionData = new byte[] { 0x19, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };
    private static readonly ulong[] ULongBuffer = new ulong[6];

    [Benchmark]
    public ulong ReadULongCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(ULongCollectionData);

        reader.ReadULongCollection(WireType.String, ULongBuffer);

        return ULongBuffer[0];
    }

    private static readonly byte[] ULongSingleItemCollection = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };

    [Benchmark]
    public ulong ReadULongSingleItemCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(ULongSingleItemCollection);

        reader.ReadULongCollection(WireType.VarInt, ULongBuffer);

        return ULongBuffer[0];
    }

    private static readonly byte[] IntCollectionData = new byte[] { 0x14, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0x0F };
    private static readonly int[] IntBuffer = new int[6];

    [Benchmark]
    public int ReadIntCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(IntCollectionData);

        reader.ReadIntCollection(WireType.String, IntBuffer);

        return IntBuffer[0];
    }

    private static readonly byte[] IntSingleItemCollection = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0x0F };

    [Benchmark]
    public int ReadIntSingleItemCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(IntSingleItemCollection);

        reader.ReadIntCollection(WireType.VarInt, IntBuffer);

        return IntBuffer[0];
    }

    private static readonly byte[] LongCollectionData = new byte[] { 0x1E, 0x00, 0x80, 0x01, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x01, 0x80, 0x80, 0x80, 0x80, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };
    private static readonly long[] LongBuffer = new long[6];

    [Benchmark]
    public long ReadLongCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(LongCollectionData);

        reader.ReadLongCollection(WireType.String, LongBuffer);

        return LongBuffer[0];
    }

    private static readonly byte[] LongSingleItemCollection = new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01 };

    [Benchmark]
    public long ReadLongSingleItemCollectionIntoBuffer()
    {
        var reader = PbfBlockReader.Create(LongSingleItemCollection);

        reader.ReadLongCollection(WireType.VarInt, LongBuffer);

        return LongBuffer[0];
    }
}