using System;
using BenchmarkDotNet.Running;
using PbfLite.Benchmark.PbfLite;

namespace PbfLite.Benchmark {
    class Program {
        static void Main(string[] args) {
            BenchmarkRunner.Run<PrimitivesReadBenchmarks>();
            //BenchmarkRunner.Run<ReadTests>();
            //BenchmarkRunner.Run<ReadTestsPrepared>();

            //BenchmarkRunner.Run<WriteTests>();
        }
    }
}
