using System;
using System.Collections.Generic;

namespace PbfLite;

public partial class PbfStreamReader
{
    private delegate T ItemReaderDelegate<T>(PbfStreamReader reader);

    private List<T> ReadScalarCollection<T>(WireType wireType, WireType itemWireType, ItemReaderDelegate<T> itemReader)
    {
        var collection = new List<T>();
        
        if (wireType == WireType.String)
        {
            uint byteLength = ReadVarInt32();
            long endPosition = _stream.Position + byteLength;

            // Estimate capacity based on wire type to reduce List reallocations
            int estimatedItemSize = itemWireType switch
            {
                WireType.VarInt => 2,      // Minimum 1 byte per varint, average ~1-5
                WireType.Fixed32 => 4,     // Always 4 bytes
                WireType.Fixed64 => 8,     // Always 8 bytes
                _ => 1
            };
            int estimatedCapacity = Math.Max(1, (int)(byteLength / estimatedItemSize));
            collection.Capacity = estimatedCapacity;

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

        return collection;
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
    /// <returns>A list of decoded unsigned 32-bit integers.</returns>
    public List<uint> ReadUIntCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadUintDelegate);

    /// <summary>
    /// Reads a collection of unsigned 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded unsigned 64-bit integers.</returns>
    public List<ulong> ReadULongCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadULongDelegate);

    /// <summary>
    /// Reads a collection of signed 32-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded signed 32-bit integers.</returns>
    public List<int> ReadIntCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadIntDelegate);

    /// <summary>
    /// Reads a collection of signed 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded signed 64-bit integers.</returns>
    public List<long> ReadLongCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadLongDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 32-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded zigzag-encoded signed 32-bit integers.</returns>
    public List<int> ReadSignedIntCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadSignedIntDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 64-bit integers from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded zigzag-encoded signed 64-bit integers.</returns>
    public List<long> ReadSignedLongCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadSignedLongDelegate);

    /// <summary>
    /// Reads a collection of boolean values from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded boolean values.</returns>
    public List<bool> ReadBooleanCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.VarInt, ReadBooleanDelegate);

    /// <summary>
    /// Reads a collection of 32-bit floats from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded 32-bit floats.</returns>
    public List<float> ReadSingleCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.Fixed32, ReadSingleDelegate);

    /// <summary>
    /// Reads a collection of 64-bit floats from the stream.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of decoded 64-bit floats.</returns>
    public List<double> ReadDoubleCollection(WireType wireType) =>
        ReadScalarCollection(wireType, WireType.Fixed64, ReadDoubleDelegate);
}
