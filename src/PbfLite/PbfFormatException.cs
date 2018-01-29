using System;
using System.Collections.Generic;
using System.Text;

namespace PbfLite {
    public class PbfFormatException : Exception {
        public PbfFormatException() {
        }

        public PbfFormatException(string message)
            : base(message) {
        }

        public PbfFormatException(string message, Exception inner)
            : base(message, inner) {
        }
    }
}
