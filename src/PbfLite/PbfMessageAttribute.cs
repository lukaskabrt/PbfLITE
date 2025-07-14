using System;

namespace PbfLite.Contracts
{

    /// <summary>
    /// Marks target type for generation of Pbf serializer.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PbfMessageAttribute : Attribute
    { }
}