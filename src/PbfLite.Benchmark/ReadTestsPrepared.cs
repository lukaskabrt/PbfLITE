using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace PbfLite.Benchmark {
    [SimpleJob()]
    [MemoryDiagnoser]
    public class ReadTestsPrepared {
        private byte[] _data;
        private MemoryStream _dataStream;

        [GlobalSetup]
        public void LoadData() {
            _dataStream = TestsHelpers.LoadAddressBookData();
            _data = _dataStream.ToArray();

            ProtoBuf.Serializer.PrepareSerializer<Models.AddressBook>();
        }

        [Benchmark]
        public int ProtobufNetReadAddressBookWithPreparedSerializer() {
            _dataStream.Seek(0, SeekOrigin.Begin);
            var data = ProtobufNet.AddressBookSerializer.Deserialize(_dataStream);
            return data.People.Count;
        }
    }
}
