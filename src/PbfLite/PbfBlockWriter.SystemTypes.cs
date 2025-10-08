using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite;

public ref partial struct PbfBlockWriter
{
    private static readonly Encoding encoding = Encoding.UTF8;

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
        WriteVarInt32(PbfBlockWriter.Zig(value));
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
        WriteVarInt64(PbfBlockWriter.Zig(value));
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