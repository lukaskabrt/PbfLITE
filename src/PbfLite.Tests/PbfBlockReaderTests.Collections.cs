using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    private delegate void ReadCollectionDelegate<T>(PbfBlockReader reader, WireType wireType, T collection);

    public class CollectionsToBuffer : PbfReaderCollectionsTests
    {
        private static ReadOnlySpan<T> ReadCollection<T>(byte[] data, WireType wireType, int itemCount, ReadCollectionDelegate<T[]> readAction)
        {
            var buffer = new T[itemCount];
            var reader = PbfBlockReader.Create(data);
            readAction(reader, wireType, buffer);
            return buffer;
        }

        protected override ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<uint>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadUIntCollection(wireType, buffer));

        protected override ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<ulong>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadULongCollection(wireType, buffer));

        protected override ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadIntCollection(wireType, buffer));

        protected override ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadSignedIntCollection(wireType, buffer));

        protected override ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadLongCollection(wireType, buffer));

        protected override ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadSignedLongCollection(wireType, buffer));

        protected override ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<bool>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadBooleanCollection(wireType, buffer));

        protected override ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<float>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadSingleCollection(wireType, buffer));

        protected override ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<double>(data, wireType, itemCount, (reader, wireType, buffer) => reader.ReadDoubleCollection(wireType, buffer));
    }

    public class CollectionsToList : PbfReaderCollectionsTests
    {
        private static ReadOnlySpan<T> ReadCollection<T>(byte[] data, WireType wireType, int itemCount, ReadCollectionDelegate<List<T>> readAction)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<T>(itemCount);
            readAction(reader, wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<uint>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadUIntCollection(wireType, list));

        protected override ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<ulong>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadULongCollection(wireType, list));

        protected override ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadIntCollection(wireType, list));

        protected override ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<int>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadSignedIntCollection(wireType, list));

        protected override ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadLongCollection(wireType, list));

        protected override ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<long>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadSignedLongCollection(wireType, list));

        protected override ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<bool>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadBooleanCollection(wireType, list));

        protected override ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<float>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadSingleCollection(wireType, list));

        protected override ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount) =>
            ReadCollection<double>(data, wireType, itemCount, (reader, wireType, list) => reader.ReadDoubleCollection(wireType, list));
    }
}