using System;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    public delegate void ItemWriterDelegate<T>(ref PbfBlockWriter writer, T item);

    public void WriteScalarCollection<T>(ReadOnlySpan<T> items, ItemWriterDelegate<T> itemWriter)
    {
        // Write the length-prefixed content
        var lengthPosition = _position;
        WriteVarInt32(0); // Placeholder for length that will be overwritten
        var contentStart = _position;

        foreach (var item in items)
        {
            itemWriter(ref this, item);
        }

        var contentLength = _position - contentStart;

        // Calculate how many bytes the actual length will take
        var actualLengthBytes = GetVarInt32ByteCount((uint)contentLength);
        var placeholderLengthBytes = _position - lengthPosition - contentLength;

        if (actualLengthBytes != placeholderLengthBytes)
        {
            // Need to shift content to accommodate different length prefix size
            var content = _block.Slice(contentStart, contentLength);
            var newContentStart = lengthPosition + actualLengthBytes;

            if (actualLengthBytes > placeholderLengthBytes)
            {
                // Need more space, shift content right
                content.CopyTo(_block.Slice(newContentStart));
            }
            else
            {
                // Need less space, shift content left
                for (int i = 0; i < contentLength; i++)
                {
                    _block[newContentStart + i] = content[i];
                }
            }

            _position = newContentStart + contentLength;
        }

        // Write the actual length at the correct position
        WriteVarInt32At(lengthPosition, (uint)contentLength);
    }

    private int GetVarInt32ByteCount(uint value)
    {
        int count = 1;
        while (value >= 0x80)
        {
            count++;
            value >>= 7;
        }
        return count;
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
        WriteScalarCollection(items, WriteBooleanDelegate);
    }

    public void WriteSingleCollection(ReadOnlySpan<float> items)
    {
        WriteScalarCollection(items, WriteSingleDelegate);
    }

    public void WriteDoubleCollection(ReadOnlySpan<double> items)
    {
        WriteScalarCollection(items, WriteDoubleDelegate);
    }
}