using System;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    private delegate void ItemWriterDelegate<T>(ref PbfBlockWriter writer, T item);

    private void WriteScalarCollection<T>(ReadOnlySpan<T> items, ItemWriterDelegate<T> itemWriter)
    {
        var block = StartLengthPrefixedBlock(items.Length);

        foreach (var item in items)
        {
            itemWriter(ref this, item);
        }

        FinalizeLengthPrefixedBlock(block);
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

    /// <summary>
    /// Writes a collection of unsigned 32-bit integers as a length-prefixed packed field.
    /// </summary>
    /// <param name="items">The items to write.</param>
    public void WriteUIntCollection(ReadOnlySpan<uint> items)
    {
        WriteScalarCollection(items, WriteUintDelegate);
    }

    /// <summary>
    /// Writes a collection of unsigned 64-bit integers as a length-prefixed packed field.
    /// </summary>
    public void WriteULongCollection(ReadOnlySpan<ulong> items)
    {
        WriteScalarCollection(items, WriteULongDelegate);
    }

    /// <summary>
    /// Writes a collection of 32-bit integers as a length-prefixed packed field.
    /// </summary>
    public void WriteIntCollection(ReadOnlySpan<int> items)
    {
        WriteScalarCollection(items, WriteIntDelegate);
    }

    /// <summary>
    /// Writes a collection of 64-bit integers as a length-prefixed packed field.
    /// </summary>
    public void WriteLongCollection(ReadOnlySpan<long> items)
    {
        WriteScalarCollection(items, WriteLongDelegate);
    }

    /// <summary>
    /// Writes a collection of zigzag-encoded signed 32-bit integers as a packed field.
    /// </summary>
    public void WriteSignedIntCollection(ReadOnlySpan<int> items)
    {
        WriteScalarCollection(items, WriteSignedIntDelegate);
    }

    /// <summary>
    /// Writes a collection of zigzag-encoded signed 64-bit integers as a packed field.
    /// </summary>
    public void WriteSignedLongCollection(ReadOnlySpan<long> items)
    {
        WriteScalarCollection(items, WriteSignedLongDelegate);
    }

    /// <summary>
    /// Writes a collection of boolean values as a length-prefixed packed field.
    /// </summary>
    public void WriteBooleanCollection(ReadOnlySpan<bool> items)
    {
        WriteScalarCollection(items, WriteBooleanDelegate, items.Length);
    }

    /// <summary>
    /// Writes a collection of 32-bit floats as a length-prefixed packed field.
    /// </summary>
    public void WriteSingleCollection(ReadOnlySpan<float> items)
    {
        WriteScalarCollection(items, WriteSingleDelegate, items.Length * 4);
    }

    /// <summary>
    /// Writes a collection of 64-bit floats as a length-prefixed packed field.
    /// </summary>
    public void WriteDoubleCollection(ReadOnlySpan<double> items)
    {
        WriteScalarCollection(items, WriteDoubleDelegate, items.Length * 8);
    }
}