using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlock
{
    private const long Int64Msb = ((long)1) << 63;
    private const int Int32Msb = ((int)1) << 31;

    private Span<byte> _block;
    private int _position;

    public readonly int Position => _position;

    public Span<byte> Block => _block.Slice(0, _position);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PbfBlock Create(Span<byte> block)
    {
        var result = new PbfBlock
        {
            _block = block,
            _position = 0
        };

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Zag(uint ziggedValue)
    {
        int value = (int)ziggedValue;
        return (-(value & 0x01)) ^ ((value >> 1) & ~PbfBlock.Int32Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Zag(ulong ziggedValue)
    {
        var value = (long)ziggedValue;
        return (-(value & 0x01L)) ^ ((value >> 1) & ~PbfBlock.Int64Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int fieldNumber, WireType wireType) ReadFieldHeader()
    {
        if (_position == _block.Length)
        {
            return (0, WireType.None);
        }

        var header = ReadVarint32();
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
            case WireType.Variant: ReadVarint64(); break;
            case WireType.Fixed32: _position += 4; break;
            case WireType.Fixed64: _position += 8; break;
            case WireType.String:
                var length = ReadVarint64();
                _position += (int)length;
                break;
            default: throw new ArgumentException($"Unable to skip field. '{wireType}' is unknown  WireType");
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadFixed32()
    {
        return ((uint)_block[_position++])
            | (((uint)_block[_position++]) << 8)
            | (((uint)_block[_position++]) << 16)
            | (((uint)_block[_position++]) << 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFixed32(uint value)
    {
        _block[_position++] = (byte)(value & 0xFF);
        _block[_position++] = (byte)((value >> 8) & 0xFF);
        _block[_position++] = (byte)((value >> 16) & 0xFF);
        _block[_position++] = (byte)((value >> 24) & 0xFF);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64()
    {
        return ((ulong)_block[_position++])
            | (((ulong)_block[_position++]) << 8)
            | (((ulong)_block[_position++]) << 16)
            | (((ulong)_block[_position++]) << 24)
            | (((ulong)_block[_position++]) << 32)
            | (((ulong)_block[_position++]) << 40)
            | (((ulong)_block[_position++]) << 48)
            | (((ulong)_block[_position++]) << 56);
    }

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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadVarint32()
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
    public void WriteVarint32(uint value)
    {
        while (value >= 0x80)
        {
            _block[_position++] = (byte)(value | 0x80);
            value >>= 7;
        }
        _block[_position++] = (byte)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadVarint64()
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
    public Span<byte> ReadLengthPrefixedBytes()
    {
        var length = ReadVarint32();
        _position += (int)length;
        return _block.Slice(_position - (int)length, (int)length);
    }
}
