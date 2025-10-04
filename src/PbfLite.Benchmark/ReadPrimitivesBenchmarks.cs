using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PbfLite.Benchmark;

public class ReadPrimitivesBenchmarks
{
    private static readonly byte[] Fixed32Data = new byte[] { 0x01, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF };

    [Benchmark]
    public uint ReadFixed32()
    {
        var block = PbfBlock.Create(Fixed32Data);
        block.ReadFixed32();
        block.ReadFixed32();
        block.ReadFixed32();
        block.ReadFixed32();
        block.ReadFixed32();
        return block.ReadFixed32();
    }
}
