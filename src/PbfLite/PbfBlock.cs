using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite; 

public ref partial struct PbfBlock {
    private const long Int64Msb = ((long)1) << 63;
    private const int Int32Msb = ((int)1) << 31;

    public int Position { get; set; }
    public int Length { get; set; }
    public ReadOnlySpan<byte> Block { get; set; }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PbfBlock Create(ReadOnlySpan<byte> block) {
        var result = new PbfBlock {
            Block = block,
            Position = 0,
            Length = block.Length
        };

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Zag(uint ziggedValue) {
        int value = (int)ziggedValue;
        return (-(value & 0x01)) ^ ((value >> 1) & ~PbfBlock.Int32Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long Zag(ulong ziggedValue) {
        var value = (long)ziggedValue;
        return (-(value & 0x01L)) ^ ((value >> 1) & ~PbfBlock.Int64Msb);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public (int fieldNumber, WireType wireType) ReadFieldHeader() {
        if (this.Position == this.Length) {
            return (0, WireType.None);
        }

        var header = this.ReadVarint32();
        if (header != 0) {
            return ((int)(header >> 3), (WireType)(header & 7));
        } else {
            return (0, WireType.None);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SkipField(WireType wireType) {
        switch (wireType) {
            case WireType.Variant: this.ReadVarint64(); break;
            case WireType.Fixed32: this.Position += 4; break;
            case WireType.Fixed64: this.Position += 8; break;
            case WireType.String:
                var length = this.ReadVarint64();
                this.Position += (int)length;
                break;
            default: throw new ArgumentException($"Unable to skip field. '{wireType}' is unknown  WireType");
        }
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadFixed32() {
        return ((uint)this.Block[this.Position++])
            | (((uint)this.Block[this.Position++]) << 8)
            | (((uint)this.Block[this.Position++]) << 16)
            | (((uint)this.Block[this.Position++]) << 24);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadFixed64() {
        return ((ulong)this.Block[this.Position++])
            | (((ulong)this.Block[this.Position++]) << 8)
            | (((ulong)this.Block[this.Position++]) << 16)
            | (((ulong)this.Block[this.Position++]) << 24)
            | (((ulong)this.Block[this.Position++]) << 32)
            | (((ulong)this.Block[this.Position++]) << 40)
            | (((ulong)this.Block[this.Position++]) << 48)
            | (((ulong)this.Block[this.Position++]) << 56);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadVarint32() {
        uint value = this.Block[this.Position++];
        if ((value & 0x80) == 0) {
            return value;
        }
        value &= 0x7F;

        uint chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= chunk << 28; // can only use 4 bits from this chunk
        if ((chunk & 0xF0) == 0) {
            return value;
        }

        if ((chunk & 0xF0) == 0xF0 &&
            this.Block[this.Position++] == 0xFF &&
            this.Block[this.Position++] == 0xFF &&
            this.Block[this.Position++] == 0xFF &&
            this.Block[this.Position++] == 0xFF &&
            this.Block[this.Position++] == 0x01) {
            return value;
        }

        throw new InvalidOperationException("Malformed  VarInt");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadVarint64() {
        ulong value = this.Block[this.Position++];
        if ((value & 0x80) == 0) {
            return value;
        }
        value &= 0x7F;

        ulong chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 7;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 14;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 21;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 28;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 35;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 42;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 49;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= (chunk & 0x7F) << 56;
        if ((chunk & 0x80) == 0) {
            return value;
        }

        chunk = this.Block[this.Position++];
        value |= chunk << 63;

        if ((chunk & ~(ulong)0x01) != 0) {
            throw new InvalidOperationException("Malformed  VarInt");
        }

        return value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<byte> ReadLengthPrefixedBytes() {
        var length = this.ReadVarint32();
        this.Position += (int)length;
        return this.Block.Slice(this.Position - (int)length, (int)length);
    }
}
