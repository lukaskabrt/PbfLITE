using BenchmarkDotNet.Attributes;
using System.IO;

namespace PbfLite.Benchmark;

[MemoryDiagnoser]
public class ReadTestsPrepared
{
    private MemoryStream _dataStream;

    [GlobalSetup]
    public void LoadData()
    {
        _dataStream = TestsHelpers.LoadAddressBookData();

        ProtoBuf.Serializer.PrepareSerializer<Models.AddressBook>();
    }

    [Benchmark]
    public int ProtobufNetReadAddressBookWithPreparedSerializer()
    {
        _dataStream.Seek(0, SeekOrigin.Begin);
        var data = ProtobufNet.AddressBookSerializer.Deserialize(_dataStream);
        return data.People.Count;
    }
}
