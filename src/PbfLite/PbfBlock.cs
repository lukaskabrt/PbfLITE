using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace PbfLite
{
    public ref partial struct PbfBlock
    {
        private const long Int64Msb = ((long)1) << 63;
        private const int Int32Msb = ((int)1) << 31;

        public int Position { get; set; }
        public int Length { get; set; }
        public ReadOnlySpan<byte> Block { get; set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PbfBlock Create(ReadOnlySpan<byte> block)
        {
            var result = new PbfBlock
            {
                Block = block,
                Position = 0,
                Length = block.Length
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
            if (this.Position == this.Length)
            {
                return (0, WireType.None);
            }

            var header = this.ReadVarint32();
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
        public uint ReadFixed32()
        {
            uint value = BinaryPrimitives.ReadUInt32LittleEndian(Block.Slice(Position, 4));
            Position += 4;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadFixed64()
        {
            ulong value = BinaryPrimitives.ReadUInt64LittleEndian(Block.Slice(Position, 8));
            Position += 8;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        private static int IndexOfFirstZero(Vector<byte> vector)
        {
            var cmp = Sse2.CompareEqual(vector.AsVector128(), Vector128<byte>.Zero); // 0xFF where zero, 0x00 where non-zero
            int mask = Sse2.MoveMask(cmp); // 16 bits: 1 if zero, 0 if non-zero

            // Invert mask: 1 if non-zero, 0 if zero
            mask = mask & 0xFFFF;
            return BitOperations.TrailingZeroCount(mask);
        }

        private static Vector<byte> VarIntMask = new Vector<byte>(0x80);
        private static Vector<byte> VarIntDataMask = new Vector<byte>(0x7F);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public uint ReadVarint32()
        {
            var available = Length - Position;
            if (Vector.IsHardwareAccelerated && available >= Vector<byte>.Count)
            {
                //Span<byte> buffer = stackalloc byte[Vector<byte>.Count];
                //Block.Slice(Position, 5).CopyTo(buffer);

                var vector = new Vector<byte>(Block.Slice(Position, Vector<byte>.Count));
                var cmp = Vector.BitwiseAnd(vector, VarIntMask);
                var data = Vector.BitwiseAnd(vector, VarIntDataMask);

                var index = IndexOfFirstZero(cmp);
                switch (index)
                {
                    case 0:
                        Position += 1;
                        return data[0];
                    case 1:
                        Position += 2;
                        return (uint)(data[0] | (data[1] << 7));
                    case 2:
                        Position += 3;
                        return (uint)(data[0] | (data[1] << 7) | (data[2] << 14));
                    case 3:
                        Position += 4;
                        return (uint)(data[0] | (data[1] << 7) | (data[2] << 14) | (data[3] << 21));
                    case 4:
                        Position += 5;
                        return (uint)(data[0] | (data[1] << 7) | (data[2] << 14) | (data[3] << 21) | (data[4] << 28));
                }
                //for (int i = 0; i < Math.Min(5, loadSize); i++)
                //{
                //    if (cmp[i] == 0)
                //    {
                //        uint value = 0;
                //        for (int j = 0; j <= i; j++)
                //        {
                //            uint chunk = this.Block[this.Position++];
                //            value |= (chunk & 0x7Fu) << (7 * j);
                //            if ((chunk & 0x80) == 0)
                //                return value;
                //        }
                //        throw new InvalidOperationException("Malformed VarInt");
                //    }
                //}
                // If not found in first 5 bytes, fall through to scalar fallback
            }

            uint value = this.Block[this.Position++];
            if ((value & 0x80) == 0)
            {
                return value;
            }
            value &= 0x7F;

            uint chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 7;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 14;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 21;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= chunk << 28; // can only use 4 bits from this chunk
            if ((chunk & 0xF0) == 0)
            {
                return value;
            }

            if ((chunk & 0xF0) == 0xF0 &&
                this.Block[this.Position++] == 0xFF &&
                this.Block[this.Position++] == 0xFF &&
                this.Block[this.Position++] == 0xFF &&
                this.Block[this.Position++] == 0xFF &&
                this.Block[this.Position++] == 0x01)
            {
                return value;
            }

            throw new InvalidOperationException("Malformed  VarInt");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadVarint64()
        {
            ulong value = this.Block[this.Position++];
            if ((value & 0x80) == 0)
            {
                return value;
            }
            value &= 0x7F;

            ulong chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 7;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 14;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 21;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 28;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 35;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 42;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 49;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
            value |= (chunk & 0x7F) << 56;
            if ((chunk & 0x80) == 0)
            {
                return value;
            }

            chunk = this.Block[this.Position++];
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
            var length = this.ReadVarint32();
            this.Position += (int)length;
            return this.Block.Slice(this.Position - (int)length, (int)length);
        }
    }
}
