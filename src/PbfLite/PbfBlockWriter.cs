using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    private Span<byte> _block;
    private int _position;

    public readonly int Position => _position;

    public Span<byte> Block => _block.Slice(0, _position);

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

    public void WriteFieldHeader(int fieldNumber, WireType wireType)
    {
        var header = ((uint)fieldNumber << 3) | (uint)wireType;
        WriteVarInt32(header);
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
    public void WriteLengthPrefixedBytes(ReadOnlySpan<byte> data)
    {
        WriteVarInt32((uint)data.Length);

        data.CopyTo(_block[_position..]);
        _position += data.Length;
    }
}