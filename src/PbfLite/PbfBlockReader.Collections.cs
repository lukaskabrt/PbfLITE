using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlockReader
{
    public delegate T ItemReaderDelegate<T>(ref PbfBlockReader reader);

    public Span<T> ReadCollection<T>(WireType wireType, WireType itemWireType, Span<T> buffer, ItemReaderDelegate<T> itemReader)
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

    public Span<uint> ReadUIntCollection(WireType wireType, Span<uint> buffer)
    {
        return ReadCollection(wireType, WireType.VarInt, buffer, ReadUintDelegate);
    }

    public Span<ulong> ReadULongCollection(WireType wireType, Span<ulong> buffer)
    {
        return ReadCollection(wireType, WireType.VarInt, buffer, ReadULongDelegate);
    }

    public Span<int> ReadIntCollection(WireType wireType, Span<int> buffer)
    {
        return ReadCollection(wireType, WireType.VarInt, buffer, ReadIntDelegate);
    }

    public Span<long> ReadLongCollection(WireType wireType, Span<long> buffer)
    {
        return ReadCollection(wireType, WireType.VarInt, buffer, ReadLongDelegate);
    }
}