namespace PbfLite; 

/// <summary>
/// Indicates the encoding used to represent an individual value in a Protocol Buffers stream.
/// </summary>
public enum WireType {
    /// <summary>
    /// Represents an error condition
    /// </summary>
    None = -1,

    /// <summary>
    /// Base-128 variable-width integers.
    /// </summary>
    VarInt = 0,

    /// <summary>
    /// Fixed-length 8-byte encoding.
    /// </summary>
    Fixed64 = 1,

    /// <summary>
    /// Length-prefixed encoding.
    /// </summary>
    String = 2,

    /// <summary>
    /// Fixed-length 4-byte encoding
    /// </summary>
    Fixed32 = 5
}
