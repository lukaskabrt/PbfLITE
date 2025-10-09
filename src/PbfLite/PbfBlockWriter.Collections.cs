using System;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    public delegate void ItemWriterDelegate<T>(ref PbfBlockWriter writer, T item);
    
    private void WriteScalarCollection<T>(ReadOnlySpan<T> items, ItemWriterDelegate<T> itemWriter)
    {
        var lengthPosition = _position;

        // Placeholder for length that will be overwritten
        WriteVarInt32(0); 

        foreach (var item in items)
        {
            itemWriter(ref this, item);
        }

        var contentLength = _position - lengthPosition - 1;
        var contentLengthBytesCount = GetVarIntBytesCount((uint)contentLength);

        if (contentLengthBytesCount > 1)
        {
            var content = _block.Slice(lengthPosition + 1, contentLength);
            var newContentStart = lengthPosition + contentLengthBytesCount;

            content.CopyTo(_block.Slice(newContentStart));
            
            _position = newContentStart + contentLength;
        }

        WriteVarInt32At(lengthPosition, (uint)contentLength);
    }

    private void WriteScalarCollection<T>(ReadOnlySpan<T> items, ItemWriterDelegate<T> itemWriter, int contentLengthBytes)
    {
        WriteVarInt32((uint)contentLengthBytes);

        foreach (var item in items)
        {
            itemWriter(ref this, item);
        }
    }

    private void WriteVarInt32At(int position, uint value)
    {
        var originalPosition = _position;
        _position = position;
        WriteVarInt32(value);
        _position = originalPosition;
    }

    private static readonly ItemWriterDelegate<uint> WriteUintDelegate = static (ref PbfBlockWriter writer, uint item) => writer.WriteUint(item);
    private static readonly ItemWriterDelegate<ulong> WriteULongDelegate = static (ref PbfBlockWriter writer, ulong item) => writer.WriteULong(item);
    private static readonly ItemWriterDelegate<int> WriteIntDelegate = static (ref PbfBlockWriter writer, int item) => writer.WriteInt(item);
    private static readonly ItemWriterDelegate<long> WriteLongDelegate = static (ref PbfBlockWriter writer, long item) => writer.WriteLong(item);
    private static readonly ItemWriterDelegate<int> WriteSignedIntDelegate = static (ref PbfBlockWriter writer, int item) => writer.WriteSignedInt(item);
    private static readonly ItemWriterDelegate<long> WriteSignedLongDelegate = static (ref PbfBlockWriter writer, long item) => writer.WriteSignedLong(item);
    private static readonly ItemWriterDelegate<bool> WriteBooleanDelegate = static (ref PbfBlockWriter writer, bool item) => writer.WriteBoolean(item);
    private static readonly ItemWriterDelegate<float> WriteSingleDelegate = static (ref PbfBlockWriter writer, float item) => writer.WriteSingle(item);
    private static readonly ItemWriterDelegate<double> WriteDoubleDelegate = static (ref PbfBlockWriter writer, double item) => writer.WriteDouble(item);

    public void WriteUIntCollection(ReadOnlySpan<uint> items)
    {
        WriteScalarCollection(items, WriteUintDelegate);
    }

    public void WriteULongCollection(ReadOnlySpan<ulong> items)
    {
        WriteScalarCollection(items, WriteULongDelegate);
    }

    public void WriteIntCollection(ReadOnlySpan<int> items)
    {
        WriteScalarCollection(items, WriteIntDelegate);
    }

    public void WriteLongCollection(ReadOnlySpan<long> items)
    {
        WriteScalarCollection(items, WriteLongDelegate);
    }

    public void WriteSignedIntCollection(ReadOnlySpan<int> items)
    {
        WriteScalarCollection(items, WriteSignedIntDelegate);
    }

    public void WriteSignedLongCollection(ReadOnlySpan<long> items)
    {
        WriteScalarCollection(items, WriteSignedLongDelegate);
    }

    public void WriteBooleanCollection(ReadOnlySpan<bool> items)
    {
        WriteScalarCollection(items, WriteBooleanDelegate, items.Length);
    }

    public void WriteSingleCollection(ReadOnlySpan<float> items)
    {
        WriteScalarCollection(items, WriteSingleDelegate, items.Length * 4);
    }

    public void WriteDoubleCollection(ReadOnlySpan<double> items)
    {
        WriteScalarCollection(items, WriteDoubleDelegate, items.Length * 8);
    }
}