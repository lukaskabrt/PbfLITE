using BenchmarkDotNet.Running;

namespace PbfLite.Benchmark;

class Program
{
    static void Main(string[] args)
    {
        //BenchmarkRunner.Run<PbfBlockReaderPrimitivesBenchmarks>();
        BenchmarkRunner.Run<PbfBlockReaderCollectionsBenchmarks>();
        //BenchmarkRunner.Run<ReadTests>();
        //BenchmarkRunner.Run<ReadTestsPrepared>();

        //BenchmarkRunner.Run<PbfBlockWriterPrimitivesBenchmarks>();
        //BenchmarkRunner.Run<WriteTests>();
    }
}
