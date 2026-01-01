using System;
using System.Runtime.CompilerServices;

namespace PbfLite;

/// <summary>
/// Internal helper class for shared encoding/decoding logic used by both
/// PbfBlockReader and PbfStreamReader.
/// </summary>
internal static class PbfEncodingHelpers
{
    private const long Int64Msb = ((long)1) << 63;
    private const int Int32Msb = ((int)1) << 31;

    /// <summary>
    /// Decodes a zigzag-encoded 32-bit unsigned integer to a signed integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int Zag(uint ziggedValue)
    {
        int value = (int)ziggedValue;
        return (-(value & 0x01)) ^ ((value >> 1) & ~Int32Msb);
    }

    /// <summary>
    /// Decodes a zigzag-encoded 64-bit unsigned integer to a signed integer.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static long Zag(ulong ziggedValue)
    {
        var value = (long)ziggedValue;
        return (-(value & 0x01L)) ^ ((value >> 1) & ~Int64Msb);
    }

    /// <summary>
    /// Encodes a field number and wire type into a field header varint.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static uint EncodeFieldHeader(int fieldNumber, WireType wireType)
    {
        return ((uint)fieldNumber << 3) | ((uint)wireType & 7);
    }

    /// <summary>
    /// Decodes a field header varint into field number and wire type.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static (int fieldNumber, WireType wireType) DecodeFieldHeader(uint header)
    {
        return ((int)(header >> 3), (WireType)(header & 7));
    }
}
