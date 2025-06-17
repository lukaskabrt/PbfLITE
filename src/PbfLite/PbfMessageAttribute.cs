using System;

namespace PbfLite;

/// <summary>
/// Marks target type for generation of Pbf serializer.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class PbfMessageAttribute : Attribute
{}