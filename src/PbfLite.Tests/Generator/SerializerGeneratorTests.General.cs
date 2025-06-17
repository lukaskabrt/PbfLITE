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
@"#nullable enable

namespace Test
{
    public class TestType
    {
        public string StringProperty { get; set; }
    }
}";

            // When
            var result = GenerateSources(sourceCode);

            // Then
            var runResult = Assert.Single(result.Results);
            Assert.Empty(runResult.GeneratedSources);
        }

        [Fact]
        public void GeneratesEmptyDeserializeMethodForClassWithMarkerAttribute()
        {
            // Given
            var sourceCode =
@"#nullable enable

using PbfLite;

namespace Test
{
    [PbfMessage]
    public partial class TestType
    {
        public string StringProperty { get; set; }
    }
}";

            var expectedGeneratedCode =
@"#nullable enable

namespace Test
{
    public partial class TestType
    {
        public static TestType Deserialize(PbfBlock pbf)
        {
            return new TestType();
        }
    }
}
";
            // When
            var result = GenerateSources(sourceCode);

            // Then
            AssertContainsSource("TestType.PbfLite.g.cs", expectedGeneratedCode, result);
        }
    }
}