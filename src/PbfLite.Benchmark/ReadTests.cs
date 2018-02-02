using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Attributes.Jobs;
using PbfLite.Benchmark.ProtobufNet.Models;
using ProtoBuf;

namespace PbfLite.Benchmark {
    [SimpleJob(targetCount: 15)]
    [MemoryDiagnoser]
    public class ReadTests {
        private byte[] _data;
        private MemoryStream _dataStream;

        [GlobalSetup]
        public void LoadData() {
            _data = File.ReadAllBytes("c:\\Temp\\addressbook.bin");
            _dataStream = new MemoryStream(_data);
        }

        [Benchmark]
        public int ProtobufNetReadAddressBook() {
            _dataStream.Seek(0, SeekOrigin.Begin);
            var data = Serializer.Deserialize<AddressBook>(_dataStream);
            return data.People.Count;
        }

        [Benchmark]
        public int PbfLiteReadAddressBook() {
            var data = PbfLite.AddressBookDeserializer.Deserialize(_data);
            return data.People.Count;
        }
    }
}
