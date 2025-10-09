using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace PbfLite;

/// <summary>
/// Reader for Protocol Buffers binary blocks. Provides methods to read values from a block of bytes.
/// </summary>
public ref partial struct PbfBlockReader
{
    private const long Int64Msb = ((long)1) << 63;
    private const int Int32Msb = ((int)1) << 31;

    private ReadOnlySpan<byte> _block;
    private int _position;

    /// <summary>
    /// Gets the current read position inside the block.
    /// </summary>
    public readonly int Position => _position;

    /// <summary>
    /// Creates a new <see cref="PbfBlockReader"/> instance for the provided
    /// block of bytes.
    /// </summary>
    /// <param name="block">The buffer to read from.</param>
    /// <returns>A new reader positioned at the beginning of <paramref name="block"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PbfBlockReader Create(ReadOnlySpan<byte> block)
    {
        return new PbfBlockReader
        {
            _block = block,
            _position = 0
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Zag(uint ziggedValue)
    {
        int value = (int)ziggedValue;
        return (-(value & 0x01)) ^ ((value >> 1) & ~Int32Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Zag(ulong ziggedValue)
    {
        var value = (long)ziggedValue;
        return (-(value & 0x01L)) ^ ((value >> 1) & ~Int64Msb);
    }

    /// <summary>
    /// Reads the next field header from the stream and returns the field
    /// number and wire type. If the end of the block is reached the
    /// returned field number will be zero and the wire type will be
    /// <see cref="WireType.None"/>.
    /// </summary>
    /// <returns>Tuple of (fieldNumber, wireType).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int fieldNumber, WireType wireType) ReadFieldHeader()
    {
        if (_position == _block.Length)
        {
            return (0, WireType.None);
        }

        var header = ReadVarInt32();
        if (header != 0)
        {
            return ((int)(header >> 3), (WireType)(header & 7));
        }
        else
        {
            return (0, WireType.None);
        }
    }

    /// <summary>
    /// Skips a field with the specified wire type.
    /// </summary>
    /// <param name="wireType">The wire type of the field to skip.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipField(WireType wireType)
    {
        switch (wireType)
        {
            case WireType.VarInt: ReadVarInt64(); break;
            case WireType.Fixed32: _position += 4; break;
            case WireType.Fixed64: _position += 8; break;
            case WireType.String:
                var length = ReadVarInt64();
                _position += (int)length;
                break;
            default: throw new ArgumentException($"Unable to skip field. '{wireType}' is unknown  WireType");
        }
    }

    /// <summary>
    /// Reads a 4-byte little-endian fixed value.
    /// </summary>
    /// <returns>The 32-bit unsigned integer read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadFixed32()
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(_block.Slice(_position, 4));
        _position += 4;

        return value;
    }

    /// <summary>
    /// Reads an 8-byte little-endian fixed value.
    /// </summary>
    /// <returns>The 64-bit unsigned integer read.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64()
    {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(_block.Slice(_position, 8));
        _position += 8;

        return value;
    }

    /// <summary>
    /// Reads a 32-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <returns>The decoded 32-bit unsigned integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadVarInt32()
    {
        uint value = _block[_position++];
        if ((value & 0x80) == 0)
        {
            return value;
        }
        value &= 0x7F;

        uint chunk = _block[_position++];
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= chunk << 28; // can only use 4 bits from this chunk
        if ((chunk & 0xF0) == 0)
        {
            return value;
        }

        if ((chunk & 0xF0) == 0xF0 &&
            _block[_position++] == 0xFF &&
            _block[_position++] == 0xFF &&
            _block[_position++] == 0xFF &&
            _block[_position++] == 0xFF &&
            _block[_position++] == 0x01)
        {
            return value;
        }

        throw new InvalidOperationException("Malformed varint");
    }

    /// <summary>
    /// Reads a 64-bit unsigned integer encoded as a base-128 varint.
    /// </summary>
    /// <returns>The decoded 64-bit unsigned integer.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadVarInt64()
    {
        ulong value = _block[_position++];
        if ((value & 0x80) == 0)
        {
            return value;
        }
        value &= 0x7F;

        ulong chunk = _block[_position++];
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 28;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 35;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 42;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 49;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= (chunk & 0x7F) << 56;
        if ((chunk & 0x80) == 0)
        {
            return value;
        }

        chunk = _block[_position++];
        value |= chunk << 63;

        if ((chunk & ~(ulong)0x01) != 0)
        {
            throw new InvalidOperationException("Malformed varint");
        }

        return value;
    }

    /// <summary>
    /// Reads a length-prefixed sequence of bytes and returns it as a
    /// <see cref="ReadOnlySpan{T}"/> referring to the underlying block.
    /// </summary>
    /// <returns>The slice of bytes representing the value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadLengthPrefixedBytes()
    {
        var length = ReadVarInt32();

        _position += (int)length;
        return _block.Slice(_position - (int)length, (int)length);
    }
}