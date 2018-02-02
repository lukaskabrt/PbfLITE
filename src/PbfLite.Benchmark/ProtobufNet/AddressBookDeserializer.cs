using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using PbfLite.Benchmark.Models;
using ProtoBuf;

namespace PbfLite.Benchmark.ProtobufNet {
    class AddressBookDeserializer {
        public static AddressBook Deserialize(Stream stream) {
            return Serializer.Deserialize<AddressBook>(stream);
        }
    }
}
