using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace PbfLite.Tests;

public partial class PbfStreamReaderTests
{
    public class Collections : PbfReaderCollectionsTests
    {
        private static ReadOnlySpan<T> ReadCollection<T>(byte[] data, WireType wireType, int itemCount, Action<PbfStreamReader, WireType, List<T>> readAction)
        {
            using var stream = new MemoryStream(data);
            var reader = new PbfStreamReader(stream);
            var items = new List<T>(itemCount);

            readAction(reader, wireType, items);

            return CollectionsMarshal.AsSpan(items);
        }

        protected override ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<uint>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadUIntCollection(wireType, items));

        protected override ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<ulong>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadULongCollection(wireType, items));

        protected override ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadIntCollection(wireType, items));

        protected override ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadSignedIntCollection(wireType, items));

        protected override ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadLongCollection(wireType, items));

        protected override ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadSignedLongCollection(wireType, items));

        protected override ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<bool>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadBooleanCollection(wireType, items));

        protected override ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<float>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadSingleCollection(wireType, items));

        protected override ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<double>(data, wireType, itemCount, (reader, wireType, items) => reader.ReadDoubleCollection(wireType, items));
    }
}