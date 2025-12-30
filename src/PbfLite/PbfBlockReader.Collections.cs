using System;
using System.Collections.Generic;

namespace PbfLite;

public ref partial struct PbfBlockReader
{
    private delegate T ItemReaderDelegate<T>(ref PbfBlockReader reader);

    private Span<T> ReadScalarCollection<T>(WireType wireType, WireType itemWireType, Span<T> buffer, ItemReaderDelegate<T> itemReader)
    {
        if (wireType == WireType.String)
        {
            uint byteLength = ReadVarInt32();
            var endPosition = _position + byteLength;

            var itemsCount = 0;
            while (_position < endPosition)
            {
                buffer[itemsCount++] = itemReader(ref this);
            }
            return buffer[..itemsCount];
        }
        else if (wireType == itemWireType)
        {
            buffer[0] = itemReader(ref this);
            return buffer.Slice(0, 1);
        }
        else
        {
            throw new InvalidOperationException($"Cannot read collection with wire type {wireType}");
        }
    }

    private void ReadScalarCollection<T>(WireType wireType, List<T> collection, WireType itemWireType, ItemReaderDelegate<T> itemReader)
    {
        if (wireType == WireType.String)
        {
            uint byteLength = ReadVarInt32();
            var endPosition = _position + byteLength;

            // Estimate capacity based on wire type to reduce List reallocations
            int estimatedItemSize = itemWireType switch
            {
                WireType.VarInt => 2,      // Minimum 1 byte per varint, average ~1-5
                WireType.Fixed32 => 4,      // Always 4 bytes
                WireType.Fixed64 => 8,      // Always 8 bytes
                _ => 1
            };
            int estimatedCapacity = Math.Max(1, (int)(byteLength / estimatedItemSize));

            if (collection.Capacity < collection.Count + estimatedCapacity)
            {
                collection.Capacity = collection.Count + estimatedCapacity;
            }

            while (_position < endPosition)
            {
                collection.Add(itemReader(ref this));
            }
        }
        else if (wireType == itemWireType)
        {
            collection.Add(itemReader(ref this));
        }
        else
        {
            throw new InvalidOperationException($"Cannot read collection with wire type {wireType}");
        }
    }

    private static readonly ItemReaderDelegate<uint> ReadUintDelegate = static (ref reader) => reader.ReadUint();
    private static readonly ItemReaderDelegate<ulong> ReadULongDelegate = static (ref reader) => reader.ReadULong();
    private static readonly ItemReaderDelegate<int> ReadIntDelegate = static (ref reader) => reader.ReadInt();
    private static readonly ItemReaderDelegate<long> ReadLongDelegate = static (ref reader) => reader.ReadLong();
    private static readonly ItemReaderDelegate<int> ReadSignedIntDelegate = static (ref reader) => reader.ReadSignedInt();
    private static readonly ItemReaderDelegate<long> ReadSignedLongDelegate = static (ref reader) => reader.ReadSignedLong();
    private static readonly ItemReaderDelegate<bool> ReadBooleanDelegate = static (ref reader) => reader.ReadBoolean();
    private static readonly ItemReaderDelegate<float> ReadSingleDelegate = static (ref reader) => reader.ReadSingle();
    private static readonly ItemReaderDelegate<double> ReadDoubleDelegate = static (ref reader) => reader.ReadDouble();

    /// <summary>
    /// Reads a collection of unsigned 32-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<uint> ReadUIntCollection(WireType wireType, Span<uint> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadUintDelegate);

    /// <summary>
    /// Reads a collection of unsigned 32-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of unsigned 32-bit integers read from the stream.</returns>
    public void ReadUIntCollection(WireType wireType, List<uint> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadUintDelegate);

    /// <summary>
    /// Reads a collection of unsigned 64-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<ulong> ReadULongCollection(WireType wireType, Span<ulong> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadULongDelegate);

    /// <summary>
    /// Reads a collection of unsigned 64-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of unsigned 64-bit integers read from the stream.</returns>
    public void ReadULongCollection(WireType wireType, List<ulong> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadULongDelegate);

    /// <summary>
    /// Reads a collection of signed 32-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>    
    public ReadOnlySpan<int> ReadIntCollection(WireType wireType, Span<int> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadIntDelegate);

    /// <summary>
    /// Reads a collection of signed 32-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of signed 32-bit integers read from the stream.</returns>
    public void ReadIntCollection(WireType wireType, List<int> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadIntDelegate);

    /// <summary>
    /// Reads a collection of signed 64-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<long> ReadLongCollection(WireType wireType, Span<long> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadLongDelegate);

    /// <summary>
    /// Reads a collection of signed 64-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of signed 64-bit integers read from the stream.</returns>
    public void ReadLongCollection(WireType wireType, List<long> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadLongDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 32-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<int> ReadSignedIntCollection(WireType wireType, Span<int> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadSignedIntDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 32-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of zigzag-encoded signed 32-bit integers read from the stream.</returns>
    public void ReadSignedIntCollection(WireType wireType, List<int> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadSignedIntDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 64-bit integers into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<long> ReadSignedLongCollection(WireType wireType, Span<long> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadSignedLongDelegate);

    /// <summary>
    /// Reads a collection of zigzag-encoded signed 64-bit integers from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of zigzag-encoded signed 64-bit integers read from the stream.</returns>
    public void ReadSignedLongCollection(WireType wireType, List<long> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadSignedLongDelegate);

    /// <summary>
    /// Reads a collection of boolean values into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<bool> ReadBooleanCollection(WireType wireType, Span<bool> buffer) =>
        ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadBooleanDelegate);

    /// <summary>
    /// Reads a collection of boolean values from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of boolean values read from the stream.</returns>
    public void ReadBooleanCollection(WireType wireType, List<bool> collection) =>
        ReadScalarCollection(wireType, collection, WireType.VarInt, ReadBooleanDelegate);

    /// <summary>
    /// Reads a collection of 32-bit floats into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<float> ReadSingleCollection(WireType wireType, Span<float> buffer) =>
        ReadScalarCollection(wireType, WireType.Fixed32, buffer, ReadSingleDelegate);

    /// <summary>
    /// Reads a collection of 32-bit floats from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of 32-bit floats read from the stream.</returns>
    public void ReadSingleCollection(WireType wireType, List<float> collection) =>
        ReadScalarCollection(wireType, collection, WireType.Fixed32, ReadSingleDelegate);

    /// <summary>
    /// Reads a collection of 64-bit floats into the provided buffer.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <param name="buffer">The buffer to fill with decoded items.</param>
    /// <returns>A slice of <paramref name="buffer"/> containing the read items.</returns>
    public ReadOnlySpan<double> ReadDoubleCollection(WireType wireType, Span<double> buffer) =>
        ReadScalarCollection(wireType, WireType.Fixed64, buffer, ReadDoubleDelegate);

    /// <summary>
    /// Reads a collection of 64-bit floats from the input stream using the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type used to encode the collection.</param>
    /// <returns>A list of 64-bit floats read from the stream.</returns>
    public void ReadDoubleCollection(WireType wireType, List<double> collection) =>
        ReadScalarCollection(wireType, collection, WireType.Fixed64, ReadDoubleDelegate);
}