using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlockReader
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
        return PbfBlockReader.Zag(ReadVarInt32());
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
        return PbfBlockReader.Zag(ReadVarInt64());
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
}