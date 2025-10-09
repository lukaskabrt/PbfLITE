using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    private static readonly Encoding encoding = Encoding.UTF8;

    /// <summary>
    /// Writes a UTF-8 encoded, length-prefixed string into the block.
    /// </summary>
    /// <param name="value">The string to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        var bytes = encoding.GetBytes(value);
        WriteLengthPrefixedBytes(bytes);
    }

    /// <summary>
    /// Writes a boolean encoded as a varint (0 = false, 1 = true).
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBoolean(bool value)
    {
        WriteVarInt32(value ? 1u : 0u);
    }

    /// <summary>
    /// Writes a signed 32-bit integer using zigzag encoding and varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedInt(int value)
    {
        WriteVarInt32(Zig(value));
    }

    /// <summary>
    /// Writes a 32-bit integer encoded as varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int value)
    {
        WriteVarInt32((uint)value);
    }

    /// <summary>
    /// Writes an unsigned 32-bit integer encoded as varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUint(uint value)
    {
        WriteVarInt32(value);
    }

    /// <summary>
    /// Writes a signed 64-bit integer using zigzag encoding and varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedLong(long value)
    {
        WriteVarInt64(Zig(value));
    }

    /// <summary>
    /// Writes a 64-bit integer encoded as varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLong(long value)
    {
        WriteVarInt64((ulong)value);
    }

    /// <summary>
    /// Writes an unsigned 64-bit integer encoded as varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteULong(ulong value)
    {
        WriteVarInt64(value);
    }

    /// <summary>
    /// Writes a 32-bit floating point value (IEEE 754 little-endian).
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteSingle(float value)
    {
        var bytes = *(uint*)&value;
        WriteFixed32(bytes);
    }

    /// <summary>
    /// Writes a 64-bit floating point value (IEEE 754 little-endian).
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteDouble(double value)
    {
        var bytes = *(ulong*)&value;
        WriteFixed64(bytes);
    }
}