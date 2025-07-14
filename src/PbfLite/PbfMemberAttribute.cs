using System;

namespace PbfLite.Contracts
{
    /// <summary>
    /// Declares a member to be used in Pbf serialization, using the given tag.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PbfMemberAttribute : Attribute
    {
        private readonly int _tag;

        /// <summary>
        /// Creates a new ProtoMemberAttribute instance.
        /// </summary>
        /// <param name="tag">Specifies the unique tag used to identify this member within the type.</param>
        public PbfMemberAttribute(int tag)
        {
            if (tag <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tag));
            }

            _tag = tag;
        }

        /// <summary>
        /// Data format to be used when encoding this value.
        /// </summary>
        public PbfDataFormat DataFormat { get; set; }

        /// <summary>
        /// Gets the unique tag used to identify this member within the type.
        /// </summary>
        public int Tag => _tag;

        /// <summary>
        /// Gets a value indicating whether this member is packed.
        /// This option only applies to list/array data of primitive types (int, double, etc).
        /// </summary>
        public bool IsPacked { get; set; }
    }
}