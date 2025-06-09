using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using PbfLite;

namespace PbfLite.Benchmark.PbfLite;

public class PrimitivesReadBenchmarks
{
    private static readonly byte[] Fixed32Data;
    private static readonly byte[] Fixed64Data;

    static PrimitivesReadBenchmarks()
    {
        Fixed32Data = new byte[4 * 100];
        for (int i = 0; i < 100; i++)
        {
            Fixed32Data[i * 4 + 3] = 0x01;
        }

        Fixed64Data = new byte[8 * 100];
        for (int i = 0; i < 100; i++)
        {
            Fixed64Data[i * 8 + 7] = 0x01;
        }
    }

    //[Benchmark]
    public uint ReadFixed32_Benchmark()
    {
        var block = PbfBlock.Create(Fixed32Data);
        uint sum = 0;
        for (int i = 0; i < 100; i++)
        {
            sum += block.ReadFixed32();
        }
        return sum;
    }

    //[Benchmark]
    public ulong ReadFixed64_Benchmark()
    {
        var block = PbfBlock.Create(Fixed64Data);
        ulong sum = 0;
        for (int i = 0; i < 100; i++)
        {
            sum += block.ReadFixed64();
        }
        return sum;
    }

    public static IEnumerable<byte[]> Varint32TestCases => new List<byte[]>
    {
        new byte[] { 0x7F },                            // 127
        new byte[] { 0x80, 0x01 },                      // 128
        new byte[] { 0x80, 0x80, 0x01 },                // 16384
        new byte[] { 0x80, 0x80, 0x80, 0x01 },          // 2097152
        new byte[] { 0x80, 0x80, 0x80, 0x80, 0x01 }     // 268435456
    };

    public static IEnumerable<byte[]> Varint32TestCaseBuffers
    {
        get
        {
            foreach (var testCase in Varint32TestCases)
            {
                var buffer = new byte[testCase.Length * 132];
                for (int i = 0; i < 132; i++)
                {
                    Buffer.BlockCopy(testCase, 0, buffer, i * testCase.Length, testCase.Length);
                }
                yield return buffer;
            }
        }
    }

    [Benchmark]
    [ArgumentsSource(nameof(Varint32TestCaseBuffers))]
    public uint ReadVarint32_Benchmark(byte[] buffer)
    {
        var block = PbfBlock.Create(buffer);
        uint sum = 0;
        for (int i = 0; i < 100; i++)
        {
            sum += block.ReadVarint32();
        }
        return sum;
    }
}
