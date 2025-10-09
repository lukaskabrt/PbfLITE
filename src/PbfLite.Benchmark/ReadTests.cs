using BenchmarkDotNet.Attributes;
using System.IO;

namespace PbfLite.Benchmark;

[MemoryDiagnoser]
public class ReadTests
{
    private byte[] _data;
    private MemoryStream _dataStream;

    [GlobalSetup]
    public void LoadData()
    {
        _dataStream = TestsHelpers.LoadAddressBookData();
        _data = _dataStream.ToArray();
    }

    [Benchmark]
    public int ProtobufNetReadAddressBook()
    {
        _dataStream.Seek(0, SeekOrigin.Begin);
        var data = ProtobufNet.AddressBookSerializer.Deserialize(_dataStream);
        return data.People.Count;
    }

    [Benchmark]
    public int GoogleProtobufReadAddressBook()
    {
        var data = GoogleProtobuf.AddressBook.Parser.ParseFrom(_data);
        return data.People.Count;
    }

    [Benchmark]
    public int PbfLiteReaderReadAddressBook()
    {
        var data = PbfLite.AddressBookDeserializer.Deserialize(_data);
        return data.People.Count;
    }
}
