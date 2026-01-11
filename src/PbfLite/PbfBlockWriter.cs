using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace PbfLite;

/// <summary>
/// Writer for Protocol Buffers binary blocks. Provides methods to write values into a provided byte buffer.
/// </summary>
public ref partial struct PbfBlockWriter
{
    private Span<byte> _block;
    private int _position;

    /// <summary>
    /// Gets the current position inside the block.
    /// </summary>
    public readonly int Position => _position;

    /// <summary>
    /// Gets the written portion of the underlying block.
    /// </summary>
    public Span<byte> Block => _block.Slice(0, _position);

    /// <summary>
    /// Creates a new <see cref="PbfBlockWriter"/> that writes into the
    /// provided span.
    /// </summary>
    /// <param name="block">The buffer to write into.</param>
    /// <returns>A new writer instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PbfBlockWriter Create(Span<byte> block)
    {
        return new PbfBlockWriter
        {
            _block = block,
            _position = 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint Zig(int value)
    {
        return (uint)((value << 1) ^ (value >> 31));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static ulong Zig(long value)
    {
        return (ulong)((value << 1) ^ (value >> 63));
    }

    internal static int GetVarIntBytesCount(uint value)
    {
        int bits = 32 - BitOperations.LeadingZeroCount(value | 1);
        return (bits + 6) / 7;
    }

    /// <summary>
    /// Writes a field header composed of field number and wire type.
    /// </summary>
    /// <param name="fieldNumber">The Protocol Buffers field number.</param>
    /// <param name="wireType">The wire type for the field.</param>
    public void WriteFieldHeader(int fieldNumber, WireType wireType)
    {
        var header = ((uint)fieldNumber << 3) | (uint)wireType;
        WriteVarInt32(header);
    }

    /// <summary>
    /// Writes a 4-byte little-endian fixed value.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFixed32(uint value)
    {
        _block[_position++] = (byte)(value & 0xFF);
        _block[_position++] = (byte)((value >> 8) & 0xFF);
        _block[_position++] = (byte)((value >> 16) & 0xFF);
        _block[_position++] = (byte)((value >> 24) & 0xFF);
    }

    /// <summary>
    /// Writes an 8-byte little-endian fixed value.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFixed64(ulong value)
    {
        _block[_position++] = (byte)(value & 0xFF);
        _block[_position++] = (byte)((value >> 8) & 0xFF);
        _block[_position++] = (byte)((value >> 16) & 0xFF);
        _block[_position++] = (byte)((value >> 24) & 0xFF);
        _block[_position++] = (byte)((value >> 32) & 0xFF);
        _block[_position++] = (byte)((value >> 40) & 0xFF);
        _block[_position++] = (byte)((value >> 48) & 0xFF);
        _block[_position++] = (byte)((value >> 56) & 0xFF);
    }

    /// <summary>
    /// Writes a 32-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarInt32(uint value)
    {
        while (value >= 0x80)
        {
            _block[_position++] = (byte)(value | 0x80);
            value >>= 7;
        }
        _block[_position++] = (byte)value;
    }

    /// <summary>
    /// Writes a 64-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <param name="value">The value to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVarInt64(ulong value)
    {
        while (value >= 0x80)
        {
            _block[_position++] = (byte)(value | 0x80);
            value >>= 7;
        }
        _block[_position++] = (byte)value;
    }

    /// <summary>
    /// Writes a length-prefixed span of bytes into the block.
    /// </summary>
    /// <param name="data">The bytes to write.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLengthPrefixedBytes(ReadOnlySpan<byte> data)
    {
        WriteVarInt32((uint)data.Length);

        data.CopyTo(_block[_position..]);
        _position += data.Length;
    }

    /// <summary>
    /// Writes the specified sequence of bytes directly to the underlying buffer at the current position.
    /// </summary>
    /// <param name="data">The span of bytes to write to the buffer.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteRaw(ReadOnlySpan<byte> data)
    {
        data.CopyTo(_block[_position..]);
        _position += data.Length;
    }

    /// <summary>
    /// Starts a length-prefixed block with an estimated block length.
    /// </summary>
    /// <param name="estimatedBlockLength">The estimated length of the block content.</param>
    /// <returns>A LengthPrefixedBlock struct representing the block.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public LengthPrefixedBlock StartLengthPrefixedBlock(int estimatedBlockLength)
    {
        var lengthPosition = _position;
        var contentPosition = lengthPosition + GetVarIntBytesCount((uint)estimatedBlockLength);
        _position = contentPosition;

        return new LengthPrefixedBlock
        {
            LengthPosition = lengthPosition,
            ContentPosition = contentPosition
        };
    }

    /// <summary>
    /// Finalizes a length-prefixed block by calculating its actual length and writing it at the appropriate position.
    /// </summary>
    /// <param name="block">The LengthPrefixedBlock to finalize.</param>
    public void FinalizeLengthPrefixedBlock(LengthPrefixedBlock block)
    {
        var estimatedLengthBytesCount = block.ContentPosition - block.LengthPosition;

        var contentLength = _position - block.ContentPosition;
        var contentLengthBytesCount = GetVarIntBytesCount((uint)contentLength);

        if (contentLengthBytesCount != estimatedLengthBytesCount)
        {
            var content = _block.Slice(block.ContentPosition, contentLength);
            var newContentStart = block.LengthPosition + contentLengthBytesCount;

            content.CopyTo(_block.Slice(newContentStart));

            _position = newContentStart + contentLength;
        }

        WriteVarInt32At(block.LengthPosition, (uint)contentLength);
    }
}

public record struct LengthPrefixedBlock {
    public int LengthPosition;
    public int ContentPosition;
}