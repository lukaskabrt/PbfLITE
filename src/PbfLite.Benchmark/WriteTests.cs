using BenchmarkDotNet.Attributes;
using Google.Protobuf;
using System.IO;
using System.Reflection;

namespace PbfLite.Benchmark;

[MemoryDiagnoser]
public class WriteTests
{
    private Models.AddressBook _data;
    private GoogleProtobuf.AddressBook _googleData;

    [GlobalSetup]
    public void LoadData()
    {
        var assembly = typeof(WriteTests).GetTypeInfo().Assembly;

        var serializedData = TestsHelpers.LoadAddressBookData();
        _data = ProtobufNet.AddressBookSerializer.Deserialize(serializedData);
        _googleData = GoogleProtobuf.AddressBookExtensions.Map(_data);
    }

    [Benchmark]
    public long ProtobufNetWriteAddressBook()
    {
        var stream = ProtobufNet.AddressBookSerializer.Serialize(_data);
        return stream.Length;
    }

    [Benchmark]
    public long GoogleProtobufWriteAddressBook()
    {
        var stream = new MemoryStream();
        _googleData.WriteTo(stream);
        return stream.Length;
    }
}
