using System.Collections.Generic;
using Xunit;

namespace PbfLite.Tests.Generator;

public partial class SerializerGeneratorTests
{
    public class General
    {
        [Fact]
        public void DoesNotGenerateAnyOutputForClassWithoutMarkerAttribute()
        {
            // Given
            var sourceCode =
            #region
@"#nullable enable

namespace Test
{
    public class TestType
    {
        public string StringProperty { get; set; }
    }
}";
            #endregion

            // When
            var result = GenerateSources(sourceCode);

            // Then
            var runResult = Assert.Single(result.Results);
            Assert.Empty(runResult.GeneratedSources);
        }

        [Fact]
        public Task GeneratesEmptyDeserializeMethod_IfClassDoesNotContainAnyProperty()
        {
            // Given
            var sourceCode =
            #region
@"#nullable enable

using PbfLite.Contracts;

namespace Test
{
    [PbfMessage]
    public partial class TestType
    {
        public string StringProperty { get; set; }
    }
}";
            #endregion

            // When
            var result = GenerateSources(sourceCode);

            // Then
            return VerifySourceFile("TestType.PbfLite.g.cs", result);
        }

        [Fact]
        public Task GeneratesDeserializeMethodForBuildInPrimitiveProperties()
        {
            // Given
            var sourceCode =
            #region
@"#nullable enable

using PbfLite.Contracts;
using System;

namespace Test
{
    [PbfMessage]
    public partial class TestType
    {
        [PbfMember(1)]
        public int IntProperty { get; set; }

        [PbfMember(2)]
        public uint UIntProperty { get; set; }

        [PbfMember(3)]
        public long LongProperty { get; set; }

        [PbfMember(4)]
        public ulong UIntProperty { get; set; }

        [PbfMember(5)]
        public float FloatProperty { get; set; }

        [PbfMember(6)]
        public double DoubleProperty { get; set; }

        [PbfMember(7)]
        public bool BoolProperty { get; set; }
    }
}";
            #endregion

            // When
            var result = GenerateSources(sourceCode);

            // Then
            return VerifySourceFile("TestType.PbfLite.g.cs", result);
        }
    }
}
