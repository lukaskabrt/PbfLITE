using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public partial class PbfStreamReader
{
    private static readonly Encoding encoding = Encoding.UTF8;

    /// <summary>
    /// Reads a UTF-8 encoded length-prefixed string from the stream.
    /// </summary>
    /// <returns>The decoded string.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        var buffer = ReadLengthPrefixedBytes();
        return encoding.GetString(buffer);
    }

    /// <summary>
    /// Reads a boolean encoded as a varint (0 = false, 1 = true).
    /// </summary>
    /// <returns>The boolean value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        switch (ReadVarInt32())
        {
            case 0: return false;
            case 1: return true;
            default: throw new PbfFormatException($"Data contains invalid value for boolean. Valid values are '0' and '1' encoded as varint.");
        }
    }

    /// <summary>
    /// Reads a signed 32-bit integer encoded with zigzag and varint.
    /// </summary>
    /// <returns>The decoded signed integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSignedInt()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return PbfEncodingHelpers.Zag(ReadVarInt32());
    }

    /// <summary>
    /// Reads a 32-bit integer encoded as varint.
    /// </summary>
    /// <returns>The integer value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return (int)ReadVarInt32();
    }

    /// <summary>
    /// Reads an unsigned 32-bit integer encoded as varint.
    /// </summary>
    /// <returns>The unsigned integer value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUint()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return ReadVarInt32();
    }

    /// <summary>
    /// Reads a signed 64-bit integer encoded with zigzag and varint.
    /// </summary>
    /// <returns>The decoded signed long.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadSignedLong()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return PbfEncodingHelpers.Zag(ReadVarInt64());
    }

    /// <summary>
    /// Reads a 64-bit integer encoded as varint.
    /// </summary>
    /// <returns>The long value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return (long)ReadVarInt64();
    }

    /// <summary>
    /// Reads an unsigned 64-bit integer encoded as varint.
    /// </summary>
    /// <returns>The unsigned long value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        return ReadVarInt64();
    }

    /// <summary>
    /// Reads a 32-bit floating point value (IEEE 754 little-endian).
    /// </summary>
    /// <returns>The float value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float ReadSingle()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        var value = ReadFixed32();
        return *(float*)&value;
    }

    /// <summary>
    /// Reads a 64-bit floating point value (IEEE 754 little-endian).
    /// </summary>
    /// <returns>The double value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double ReadDouble()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(PbfStreamReader));

        var value = ReadFixed64();
        return *(double*)&value;
    }
}
