using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;

namespace PbfLite.Benchmark {
    [SimpleJob(targetCount: 15)]
    [MemoryDiagnoser]
    public class ReadTests {
        private byte[] _data;
        private MemoryStream _dataStream;

        [GlobalSetup]
        public void LoadData() {
            var assembly = typeof(ReadTests).GetTypeInfo().Assembly;

            _dataStream = new MemoryStream();
            using (var stream = assembly.GetManifestResourceStream("PbfLite.Benchmark.Data.addressbook.bin")) {
                stream.CopyTo(_dataStream);
            }

            _data = _dataStream.ToArray();
        }

        [Benchmark]
        public int ProtobufNetReadAddressBook() {
            _dataStream.Seek(0, SeekOrigin.Begin);
            var data = ProtobufNet.AddressBookDeserializer.Deserialize(_dataStream);
            return data.People.Count;
        }

        [Benchmark]
        public int GoogleProtobufReadAddressBook() {
            var data = GoogleProtobuf.AddressBook.Parser.ParseFrom(_data);
            return data.People.Count;
        }

        [Benchmark]
        public int PbfLiteReadAddressBook() {
            var data = PbfLite.AddressBookDeserializer.Deserialize(_data);
            return data.People.Count;
        }
    }
}
