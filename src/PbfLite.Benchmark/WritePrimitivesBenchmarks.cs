using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PbfLite.Benchmark;

public class WritePrimitivesBenchmarks
{
    private static readonly byte[] Fixed32Buffer = new byte[6 * 4];
    private static readonly byte[] Fixed64Buffer = new byte[6 * 8];

    [Benchmark]
    public int WriteFixed32()
    {
        var block = PbfBlock.Create(Fixed32Buffer);
        block.WriteFixed32(0);
        block.WriteFixed32(1);
        block.WriteFixed32(1024*1024);
        block.WriteFixed32(uint.MaxValue - 2);
        block.WriteFixed32(uint.MaxValue - 1);
        block.WriteFixed32(uint.MaxValue);

        return block.Position;
    }

    [Benchmark]
    public int WriteFixed64()
    {
        var block = PbfBlock.Create(Fixed64Buffer);
        block.WriteFixed64(0);
        block.WriteFixed64(1);
        block.WriteFixed64(1024);
        block.WriteFixed64(ulong.MaxValue - 2);
        block.WriteFixed64(ulong.MaxValue - 1);
        block.WriteFixed64(ulong.MaxValue);

        return block.Position;
    }
}
