using System;
using System.Runtime.CompilerServices;

namespace PbfLite;

/// <summary>
/// Internal helper class for shared encoding/decoding logic used by both
/// PbfBlockReader and PbfStreamReader.
/// </summary>
internal static class PbfEncoding
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

    /// <summary>
    /// Estimates the number of items that can be represented by a buffer of the specified byte length and wire type.
    /// </summary>
    /// <param name="byteLength">The total length of the buffer, in bytes, to be analyzed for item estimation. Must be non-negative.</param>
    /// <param name="itemWireType">The wire type of the items to estimate, which determines the assumed size per item.</param>
    /// <returns>An estimated count of items that could be contained within the specified byte length for the given wire type.
    /// The value is always at least 1.</returns>
    internal static int EstimateItemsCount(uint byteLength, WireType itemWireType)
    {
        var estimatedItemSize = itemWireType switch
        {
            WireType.VarInt => 2,       // Minimum 1 byte per varint, average ~1-5
            WireType.Fixed32 => 4,      // Always 4 bytes
            WireType.Fixed64 => 8,      // Always 8 bytes
            _ => 1
        };

        return Math.Max(1, (int)(byteLength / estimatedItemSize));
    }
}
