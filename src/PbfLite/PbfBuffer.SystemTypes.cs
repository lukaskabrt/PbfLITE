using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PbfLite {
    public partial struct PbfBuffer {
        private static readonly Encoding encoding = Encoding.UTF8;

        public void WriteString(string text) {
            this.WriteLengthPrefixedBytes(encoding.GetBytes(text));
        }

        public void WriteBoolean(bool value) {
            this.WriteVarint(value ? (uint)1 : (uint)0);
        }

        public void WriteSignedInt(int value) {
            this.WriteVarint(PbfBuffer.Zig(value));
        }

        public void WriteInt(int value) {
            this.WriteVarint((ulong)value);
        }

        public void WriteSignedLong(long value) {
            this.WriteVarint(PbfBuffer.Zig(value));
        }

        public void WriteLong(long value) {
            this.WriteVarint((ulong)value);
        }

        public unsafe void WriteSingle(float value) {
            this.WriteFixed32(*(uint*)&value);
        }

        public unsafe void WriteDouble(double value) {
            this.WriteFixed64(*(ulong*)&value);
        }

    }
}
