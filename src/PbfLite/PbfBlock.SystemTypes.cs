using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite {
    public ref partial struct PbfBlock {
        private static readonly Encoding encoding = Encoding.UTF8;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString() {
            var buffer = this.ReadLengthPrefixedBytes();
            return encoding.GetString(buffer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean() {
            switch (this.ReadVarint32()) {
                case 0: return false;
                case 1: return true;
                default: throw new PbfFormatException($"Data contains invalid value for boolean. Valid values are '0' and '1' encoded as varint.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadSignedInt() {
            return PbfBlock.Zag(this.ReadVarint32());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt() {
            return (int)this.ReadVarint32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUint() {
            return this.ReadVarint32();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadSignedLong() {
            return PbfBlock.Zag(this.ReadVarint64());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong() {
            return (long)this.ReadVarint64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong() {
            return this.ReadVarint64();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe float ReadSingle() {
            var value = this.ReadFixed32();
            return *(float*)&value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe double ReadDouble() {
            var value = this.ReadFixed64();
            return *(double*)&value;
        }

    }
}
