using System;
using BenchmarkDotNet.Running;

namespace PbfLite.Benchmark {
    class Program {
        static void Main(string[] args) {
            BenchmarkRunner.Run<ReadTests>();
            BenchmarkRunner.Run<ReadTestsPrepared>();
        }
    }
}
