using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PbfLite.Generator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PbfLite.Tests.Generator;


public partial class SerializerGeneratorTests
{
    // Some types required for tests are not in System.Private.CoreLib.dll so this is used to load the respective assemblies explicitly.
    private static readonly IEnumerable<Type> RequiredTypes = new[]
    {
        typeof(object),
        typeof(string),
        typeof(int),
        typeof(List<>),
        typeof(HashSet<>),
        typeof(LinkedList<>),
        typeof(IDictionary),
        typeof(IReadOnlySet<>),
        typeof(PbfBlock)
    };

    private static void AssertContainsSource(string expectedFileName, string expectedGeneratedCode, GeneratorDriverRunResult actualRunResult)
    {
        var sources = GetGeneratedSources(actualRunResult);
        Assert.Contains(expectedFileName, sources.Keys);

        var actualGeneratedCode = sources[expectedFileName].GetText().ToString();
        Assert.Equal(expectedGeneratedCode, actualGeneratedCode);
    }

    private static IReadOnlyDictionary<string, SyntaxTree> GetGeneratedSources(GeneratorDriverRunResult runResult)
    {
        var generatorRunResult = Assert.Single(runResult.Results);
        return generatorRunResult.GeneratedSources.ToDictionary(s => s.HintName, s => s.SyntaxTree);
    }

    private static GeneratorDriverRunResult GenerateSources(params string[] sources)
    {
        return GenerateSources(sources, CreateCompilationOptions());
    }

    private static GeneratorDriverRunResult GenerateSources(IEnumerable<string> sources, CSharpCompilationOptions options)
    {
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            references: RequiredTypes.Select(t => t.Assembly).Distinct().Select(a => MetadataReference.CreateFromFile(a.Location)),
            options: options);

        var generator = new SerializerGenerator();

        var driver = CSharpGeneratorDriver.Create(generator).RunGenerators(compilation);

        return driver.GetRunResult();
    }

    private static CSharpCompilationOptions CreateCompilationOptions(
        OutputKind outputKind = OutputKind.DynamicallyLinkedLibrary,
        NullableContextOptions nullableContextOptions = NullableContextOptions.Disable)
    {
        return new CSharpCompilationOptions(outputKind, nullableContextOptions: nullableContextOptions);
    }
}