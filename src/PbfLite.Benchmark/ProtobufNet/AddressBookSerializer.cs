using PbfLite.Benchmark.Models;
using ProtoBuf;
using System.IO;

namespace PbfLite.Benchmark.ProtobufNet;

class AddressBookSerializer
{
    public static Stream Serialize(AddressBook data)
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, data);
        return stream;
    }

    public static AddressBook Deserialize(Stream stream)
    {
        return Serializer.Deserialize<AddressBook>(stream);
    }
}
