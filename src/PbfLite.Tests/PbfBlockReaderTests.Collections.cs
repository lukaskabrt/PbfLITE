using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PbfLite.Tests;

public partial class PbfBlockReaderTests
{
    public class CollectionsToBuffer : PbfReaderCollectionsTests
    {
        protected override ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new uint[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadUIntCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new ulong[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadULongCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new int[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadIntCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new int[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadSignedIntCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new long[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadLongCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new long[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadSignedLongCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new bool[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadBooleanCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new float[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadSingleCollection(wireType, buffer);

            return buffer;
        }

        protected override ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount)
        {
            var buffer = new double[itemCount];

            var reader = PbfBlockReader.Create(data);
            reader.ReadDoubleCollection(wireType, buffer);

            return buffer;
        }
    }

    public class CollectionsToList : PbfReaderCollectionsTests
    {
        protected override ReadOnlySpan<uint> ReadUIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<uint>(itemCount);

            reader.ReadUIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<ulong> ReadULongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<ulong>(itemCount);

            reader.ReadULongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<int> ReadIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<int>(itemCount);

            reader.ReadIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<int> ReadSignedIntCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<int>(itemCount);

            reader.ReadSignedIntCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<long> ReadLongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<long>(itemCount);

            reader.ReadLongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<long> ReadSignedLongCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<long>(itemCount);

            reader.ReadSignedLongCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<bool> ReadBooleanCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<bool>(itemCount);

            reader.ReadBooleanCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<float> ReadSingleCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<float>(itemCount);

            reader.ReadSingleCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }

        protected override ReadOnlySpan<double> ReadDoubleCollection(byte[] data, WireType wireType, int itemCount)
        {
            var reader = PbfBlockReader.Create(data);
            var list = new List<double>(itemCount);

            reader.ReadDoubleCollection(wireType, list);
            return CollectionsMarshal.AsSpan(list);
        }
    }
}