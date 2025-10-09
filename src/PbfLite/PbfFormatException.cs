using System;

namespace PbfLite; 

/// <summary>
/// Exception thrown when data in a protobuf block is malformed or does not
/// conform to expected encoding rules.
/// </summary>
public class PbfFormatException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="PbfFormatException"/>.
    /// </summary>
    public PbfFormatException()
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PbfFormatException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and a reference
    /// to the inner exception that is the cause of this exception.
    /// </summary>
    public PbfFormatException(string message, Exception inner)
        : base(message, inner)
    {
    }
}
