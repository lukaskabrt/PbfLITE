using System;
using System.Collections.Generic;

namespace PbfLite;

public partial class PbfStreamReader
{
    private delegate T ItemReaderDelegate<T>(PbfStreamReader reader);

    private void ReadScalarCollection<T>(WireType wireType, WireType itemWireType, List<T> collection, ItemReaderDelegate<T> itemReader)
    {
        if (wireType == WireType.String)
        {
            var byteLength = ReadVarInt32();
            var endPosition = _stream.Position + byteLength;

            var estimatedItemsCount = PbfEncoding.EstimateItemsCount(byteLength, itemWireType);
            if (collection.Capacity < collection.Count + estimatedItemsCount)
            {
                collection.Capacity = collection.Count + estimatedItemsCount;
            }

            while (_stream.Position < endPosition)
            {
                collection.Add(itemReader(this));
            }
        }
        else if (wireType == itemWireType)
        {
            collection.Add(itemReader(this));
        }
        else
        {
            throw new InvalidOperationException($"Cannot read collection with wire type {wireType}");
        }
    }

    private static readonly ItemReaderDelegate<uint> ReadUintDelegate = static (reader) => reader.ReadUint();
    private static readonly ItemReaderDelegate<ulong> ReadULongDelegate = static (reader) => reader.ReadULong();
    private static readonly ItemReaderDelegate<int> ReadIntDelegate = static (reader) => reader.ReadInt();
    private static readonly ItemReaderDelegate<long> ReadLongDelegate = static (reader) => reader.ReadLong();
    private static readonly ItemReaderDelegate<int> ReadSignedIntDelegate = static (reader) => reader.ReadSignedInt();
    private static readonly ItemReaderDelegate<long> ReadSignedLongDelegate = static (reader) => reader.ReadSignedLong();
    private static readonly ItemReaderDelegate<bool> ReadBooleanDelegate = static (reader) => reader.ReadBoolean();
    private static readonly ItemReaderDelegate<float> ReadSingleDelegate = static (reader) => reader.ReadSingle();
    private static readonly ItemReaderDelegate<double> ReadDoubleDelegate = static (reader) => reader.ReadDouble();

    /// <summary>
    /// Reads a collection of unsigned 32-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadUIntCollection(WireType wireType, List<uint> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadUintDelegate);

    /// <summary>
    /// Reads a collection of unsigned 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadULongCollection(WireType wireType, List<ulong> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadULongDelegate);

    /// <summary>
    /// Reads a collection of signed 32-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadIntCollection(WireType wireType, List<int> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadIntDelegate);

    /// <summary>
    /// Reads a collection of signed 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadLongCollection(WireType wireType, List<long> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadLongDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 32-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadSignedIntCollection(WireType wireType, List<int> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadSignedIntDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadSignedLongCollection(WireType wireType, List<long> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadSignedLongDelegate);

    /// <summary>
    /// Reads a collection of boolean values from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadBooleanCollection(WireType wireType, List<bool> collection) =>
        ReadScalarCollection(wireType, WireType.VarInt, collection, ReadBooleanDelegate);

    /// <summary>
    /// Reads a collection of 32-bit floats from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadSingleCollection(WireType wireType, List<float> collection) =>
        ReadScalarCollection(wireType, WireType.Fixed32, collection, ReadSingleDelegate);

    /// <summary>
    /// Reads a collection of 64-bit floats from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="collection">The collection to fill with decoded items.</param>
    public void ReadDoubleCollection(WireType wireType, List<double> collection) =>
        ReadScalarCollection(wireType, WireType.Fixed64, collection, ReadDoubleDelegate);
}
