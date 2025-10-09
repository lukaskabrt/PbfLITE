using System;

namespace PbfLite;

public ref partial struct PbfBlockReader
{
    public delegate T ItemReaderDelegate<T>(ref PbfBlockReader reader);

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
            return buffer.Slice(0, itemsCount);
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

    private static readonly ItemReaderDelegate<uint> ReadUintDelegate = static (ref PbfBlockReader reader) => reader.ReadUint();
    private static readonly ItemReaderDelegate<ulong> ReadULongDelegate = static (ref PbfBlockReader reader) => reader.ReadULong();
    private static readonly ItemReaderDelegate<int> ReadIntDelegate = static (ref PbfBlockReader reader) => reader.ReadInt();
    private static readonly ItemReaderDelegate<long> ReadLongDelegate = static (ref PbfBlockReader reader) => reader.ReadLong();
    private static readonly ItemReaderDelegate<int> ReadSignedIntDelegate = static (ref PbfBlockReader reader) => reader.ReadSignedInt();
    private static readonly ItemReaderDelegate<long> ReadSignedLongDelegate = static (ref PbfBlockReader reader) => reader.ReadSignedLong();
    private static readonly ItemReaderDelegate<bool> ReadBooleanDelegate = static (ref PbfBlockReader reader) => reader.ReadBoolean();
    private static readonly ItemReaderDelegate<float> ReadSingleDelegate = static (ref PbfBlockReader reader) => reader.ReadSingle();
    private static readonly ItemReaderDelegate<double> ReadDoubleDelegate = static (ref PbfBlockReader reader) => reader.ReadDouble();

    public Span<uint> ReadUIntCollection(WireType wireType, Span<uint> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadUintDelegate);
    }

    public Span<ulong> ReadULongCollection(WireType wireType, Span<ulong> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadULongDelegate);
    }

    public Span<int> ReadIntCollection(WireType wireType, Span<int> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadIntDelegate);
    }

    public Span<long> ReadLongCollection(WireType wireType, Span<long> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadLongDelegate);
    }

    public Span<int> ReadSignedIntCollection(WireType wireType, Span<int> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadSignedIntDelegate);
    }

    public Span<long> ReadSignedLongCollection(WireType wireType, Span<long> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadSignedLongDelegate);
    }

    public Span<bool> ReadBooleanCollection(WireType wireType, Span<bool> buffer)
    {
        return ReadScalarCollection(wireType, WireType.VarInt, buffer, ReadBooleanDelegate);
    }

    public Span<float> ReadSingleCollection(WireType wireType, Span<float> buffer)
    {
        return ReadScalarCollection(wireType, WireType.Fixed32, buffer, ReadSingleDelegate);
    }

    public Span<double> ReadDoubleCollection(WireType wireType, Span<double> buffer)
    {
        return ReadScalarCollection(wireType, WireType.Fixed64, buffer, ReadDoubleDelegate);
    }
}