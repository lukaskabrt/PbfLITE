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

    public Span<uint> ReadUIntCollection(WireType wireType, Span<uint> buffer)
    {
        return ReadCollection(wireType, WireType.VarInt, buffer, ReadUintDelegate);
    }
}