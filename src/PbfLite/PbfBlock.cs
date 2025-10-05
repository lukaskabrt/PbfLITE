using System;
using System.Buffers.Binary;
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
    internal static int Zag(uint ziggedValue)
    {
        int value = (int)ziggedValue;
        return (-(value & 0x01)) ^ ((value >> 1) & ~PbfBlock.Int32Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Zag(ulong ziggedValue)
    {
        var value = (long)ziggedValue;
        return (-(value & 0x01L)) ^ ((value >> 1) & ~PbfBlock.Int64Msb);
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

    public void WriteFieldHeader(int fieldNumber, WireType wireType)
    {
        var header = ((uint)fieldNumber << 3) | (uint)wireType;
        WriteVarInt32(header);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipField(WireType wireType)
    {
        switch (wireType)
        {
            case WireType.Varint: ReadVarInt64(); break;
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
    public void WriteFixed32(uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(_block.Slice(_position, 4), value);
        _position += 4;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64()
    {
        var value = BinaryPrimitives.ReadUInt64LittleEndian(_block.Slice(_position, 8));
        _position += 8;

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFixed64(ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(_block.Slice(_position, 8), value);
        _position += 8;
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
    public void WriteVarInt32(uint value)
    {
        while (value >= 0x80)
        {
            _block[_position++] = (byte)(value | 0x80);
            value >>= 7;
        }
        _block[_position++] = (byte)value;
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
    public void WriteVarInt64(ulong value)
    {
        while (value >= 0x80)
        {
            _block[_position++] = (byte)(value | 0x80);
            value >>= 7;
        }
        _block[_position++] = (byte)value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ReadLengthPrefixedBytes()
    {
        var length = ReadVarInt32();

        _position += (int)length;
        return _block.Slice(_position - (int)length, (int)length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLengthPrefixedBytes(ReadOnlySpan<byte> data)
    {
        WriteVarInt32((uint)data.Length);

        data.CopyTo(_block[_position..]);
        _position += data.Length;
    }
}
