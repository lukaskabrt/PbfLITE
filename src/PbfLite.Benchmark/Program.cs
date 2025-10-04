using System;
using BenchmarkDotNet.Running;

namespace PbfLite.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        BenchmarkRunner.Run<ReadPrimitivesBenchmarks>();
        BenchmarkRunner.Run<ReadTests>();
        BenchmarkRunner.Run<ReadTestsPrepared>();

        BenchmarkRunner.Run<WriteTests>();
    }
}
