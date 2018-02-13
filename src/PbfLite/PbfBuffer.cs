using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite {
    public partial struct PbfBuffer {
        private byte[] _buffer;

        public int Position { get; private set; }

        public Span<byte> Buffer {
            get {
                return _buffer.AsSpan().Slice(0, this.Position);
            }
        }

        public static PbfBuffer Create(int length) {
            var result = new PbfBuffer() {
                Position = 0,
                _buffer = ArrayPool<byte>.Shared.Rent(length)
            };

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFieldHeader(int fieldNumber, WireType wireType) {
            var header = (((uint)fieldNumber) << 3) | (((uint)wireType) & 7);
            this.WriteVarint(header);
        }

        public void Flush(Stream destination) {
            destination.Write(this.Buffer);
            ArrayPool<byte>.Shared.Return(_buffer);
            _buffer = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFixed32(uint value) {
            _buffer[this.Position++] = (byte)value;
            _buffer[this.Position++] = (byte)(value >> 8);
            _buffer[this.Position++] = (byte)(value >> 16);
            _buffer[this.Position++] = (byte)(value >> 24);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteFixed64(ulong value) {
            _buffer[this.Position++] = (byte)value;
            _buffer[this.Position++] = (byte)(value >> 8);
            _buffer[this.Position++] = (byte)(value >> 16);
            _buffer[this.Position++] = (byte)(value >> 24);
            _buffer[this.Position++] = (byte)(value >> 32);
            _buffer[this.Position++] = (byte)(value >> 40);
            _buffer[this.Position++] = (byte)(value >> 48);
            _buffer[this.Position++] = (byte)(value >> 56);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Zig(int value) {
            return (uint)((value << 1) ^ (value >> 31));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong Zig(long value) {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteVarint(ulong value) {
            do {
                _buffer[this.Position++] = (byte)((value & 0x7F) | 0x80);
            } while ((value >>= 7) != 0);
            _buffer[this.Position - 1] &= 0x7F;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLengthPrefixedBytes(byte[] bytes) {
            this.WriteVarint((uint)bytes.Length);
            bytes.CopyTo(_buffer, this.Position);
            this.Position += bytes.Length;
        }
    }
}
