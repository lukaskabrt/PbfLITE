using BenchmarkDotNet.Attributes;
using System;
using System.Linq;

namespace PbfLite.Benchmark;

public class PbfBlockWriterCollectionsBenchmarks
{
    private static readonly byte[] buffer = new byte[5000];
    private static readonly float[] SingleValuesSmall = new float[] { 1f, 128f, 16384f, 2097152f, 268435456f, 4294967295f };
    private static readonly float[] SingleValuesLarge = Enumerable.Range(0, 1000).Select(i => (float)i).ToArray();

    [Benchmark]
    public int WriteSmallSingleCollection()
    {
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteSingleCollection(SingleValuesSmall.AsSpan());
        return writer.Position;
    }

    [Benchmark]
    public int WriteLargeSingleCollection()
    {
        var writer = PbfBlockWriter.Create(buffer);
        writer.WriteSingleCollection(SingleValuesLarge.AsSpan());
        return writer.Position;
    }
}