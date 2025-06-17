using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace PbfLite.Generator;

[Generator]
public class SerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<PbfMessageSerializer?> serializersToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "PbfLite.PbfMessageAttribute",
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetSerializerToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        // TODO: implement the remainder of the source generator

        // Generate source code for each enum found
        context.RegisterSourceOutput(serializersToGenerate,
            static (spc, source) => Execute(source, spc));
    }

    static PbfMessageSerializer? GetSerializerToGenerate(SemanticModel semanticModel, SyntaxNode enumDeclarationSyntax)
    {
        // Get the semantic representation of the enum syntax
        if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong
            return null;
        }

        // Get the full type name of the enum e.g. Colour, 
        // or OuterClass<T>.Colour if it was nested in a generic type (for example)
        var className = classSymbol.Name;
        var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();

        //// Get all the members in the enum
        //ImmutableArray<ISymbol> enumMembers = enumSymbol.GetMembers();
        //var members = new List<string>(enumMembers.Length);

        //// Get all the fields from the enum, and add their name to the list
        //foreach (ISymbol member in enumMembers)
        //{
        //    if (member is IFieldSymbol field && field.ConstantValue is not null)
        //    {
        //        members.Add(member.Name);
        //    }
        //}

        //// Create an EnumToGenerate for use in the generation phase
        //enumsToGenerate.Add(new EnumToGenerate(enumName, members));

        //foreach (ISymbol member in enumMembers)
        //{
        //    if (member is IFieldSymbol field && field.ConstantValue is not null)
        //    {
        //        members.Add(member.Name);
        //    }
        //}

        return new PbfMessageSerializer(className, classNamespace);
    }

    static void Execute(PbfMessageSerializer? serializer, SourceProductionContext context)
    {
        if (serializer == null)
        {
            return;
        }

        var builder = new SerializerBuilder(serializer);
        var result = builder.Build();

        context.AddSource(result.FileName, SourceText.From(result.SourceCode, Encoding.UTF8));
    }
}

public static class SourceGenerationHelper
{
    public static string GenerateExtensionClass(PbfMessageSerializer enumToGenerate)
    {
        var sb = new StringBuilder();
        //        sb.Append(@"
        //namespace NetEscapades.EnumGenerators
        //{
        //    public static partial class EnumExtensions
        //    {");
        //        sb.Append(@"
        //            public static string ToStringFast(this ").Append(enumToGenerate.Name).Append(@" value)
        //                => value switch
        //                {");
        //        foreach (var member in enumToGenerate.Values)
        //        {
        //            sb.Append(@"
        //            ").Append(enumToGenerate.Name).Append('.').Append(member)
        //                .Append(" => nameof(")
        //                .Append(enumToGenerate.Name).Append('.').Append(member).Append("),");
        //        }

        //        sb.Append(@"
        //                _ => value.ToString(),
        //            };
        //");

        //        sb.Append(@"
        //    }
        //}");

        return sb.ToString();
    }
}

public record class PbfMessageSerializer
{
    public string TypeName { get; private set; }

    public string Namespace { get; private set; }

    public PbfMessageSerializer(string typeName, string @namespace)
    {
        TypeName = typeName;
        Namespace = @namespace;
    }
}
