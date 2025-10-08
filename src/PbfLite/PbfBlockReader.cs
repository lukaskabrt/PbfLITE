using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace PbfLite;

public ref partial struct PbfBlockReader
{
    private const long Int64Msb = ((long)1) << 63;
    private const int Int32Msb = ((int)1) << 31;

    private ReadOnlySpan<byte> _block;
    private int _position;

    public readonly int Position => _position;

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadFixed32()
    {
        var value = BinaryPrimitives.ReadUInt32LittleEndian(_block.Slice(_position, 4));
        _position += 4;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64()
    {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(_block.Slice(_position, 8));
        _position += 8;

        return value;
    }

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

        throw new InvalidOperationException("Malformed  VarInt");
    }

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
            throw new InvalidOperationException("Malformed  VarInt");
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadLengthPrefixedBytes()
    {
        var length = ReadVarInt32();

        _position += (int)length;
        return _block.Slice(_position - (int)length, (int)length);
    }
}