using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlock
{
    private static readonly Encoding encoding = Encoding.UTF8;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        var buffer = ReadLengthPrefixedBytes();
        return encoding.GetString(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBoolean()
    {
        switch (ReadVarInt32())
        {
            case 0: return false;
            case 1: return true;
            default: throw new PbfFormatException($"Data contains invalid value for boolean. Valid values are '0' and '1' encoded as varint.");
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadSignedInt()
    {
        return PbfBlock.Zag(ReadVarInt32());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        return (int)ReadVarInt32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUint()
    {
        return ReadVarInt32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadSignedLong()
    {
        return PbfBlock.Zag(ReadVarInt64());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadLong()
    {
        return (long)ReadVarInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadULong()
    {
        return ReadVarInt64();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe float ReadSingle()
    {
        var value = ReadFixed32();
        return *(float*)&value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe double ReadDouble()
    {
        var value = ReadFixed64();
        return *(double*)&value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        var bytes = encoding.GetBytes(value);
        WriteLengthPrefixedBytes(bytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBoolean(bool value)
    {
        WriteVarInt32(value ? 1u : 0u);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedInt(int value)
    {
        WriteVarInt32(PbfBlock.Zig(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int value)
    {
        WriteVarInt32((uint)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUint(uint value)
    {
        WriteVarInt32(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteSignedLong(long value)
    {
        WriteVarInt64(PbfBlock.Zig(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteLong(long value)
    {
        WriteVarInt64((ulong)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteULong(ulong value)
    {
        WriteVarInt64(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteSingle(float value)
    {
        var intValue = *(uint*)&value;
        WriteFixed32(intValue);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void WriteDouble(double value)
    {
        var longValue = *(ulong*)&value;
        WriteFixed64(longValue);
    }
}
