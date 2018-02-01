using System;
using BenchmarkDotNet.Running;

namespace PbfLite.Benchmark {
    class Program {
        static void Main(string[] args) {
            //var test = new ReadTests();
            //test.LoadData();
            //test.ProtobufNetReadAddressBook();

            BenchmarkRunner.Run<ReadTests>();

            //var generator = new DataGenerator();
            //generator.GenerateAddressBook("addressbook.bin");
        }
    }
}
