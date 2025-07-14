using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using PbfLite.Contracts;
using PbfLite.Generator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PbfLite.Tests.Generator;


public partial class SerializerGeneratorTests
{
    private static readonly string dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

    private static readonly ImmutableArray<MetadataReference> netReferences = ImmutableArray.Create<MetadataReference>(
      // .NET assemblies are finicky and need to be loaded in a special way.
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "netstandard.dll")),
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
      MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll"))
    );

    private static readonly IEnumerable<Type> RequiredTypes = new[]
    {
        typeof(PbfBlock),
        typeof(PbfMessageAttribute),
        typeof(PbfMemberAttribute)
    };

    private static Task VerifySourceFile(string expectedFileName, GeneratorDriverRunResult actualRunResult)
    {
        var sources = GetGeneratedSources(actualRunResult);
        Assert.Contains(expectedFileName, sources.Keys);

        return Verify(sources[expectedFileName].GetText().ToString());
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
        var dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);

        var references = RequiredTypes
            .Select(t => t.Assembly)
            .Distinct()
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: sources.Select(source => CSharpSyntaxTree.ParseText(source)),
            references: netReferences.Concat(references),
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