using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PbfLite.Generator;

[Generator]
public class SerializerGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<PbfMessageSerializer?> serializersToGenerate = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "PbfLite.Contracts.PbfMessageAttribute",
                predicate: static (s, _) => true,
                transform: static (ctx, _) => GetSerializerToGenerate(ctx.SemanticModel, ctx.TargetNode))
            .Where(static m => m is not null);

        // TODO: implement the remainder of the source generator

        // Generate source code for each enum found
        context.RegisterSourceOutput(serializersToGenerate,
            static (spc, source) => Execute(source, spc));
    }    static PbfMessageSerializer? GetSerializerToGenerate(SemanticModel semanticModel, SyntaxNode enumDeclarationSyntax)
    {
        // Get the semantic representation of the enum syntax
        if (semanticModel.GetDeclaredSymbol(enumDeclarationSyntax) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong
            return null;
        }

        var className = classSymbol.Name;
        var classNamespace = classSymbol.ContainingNamespace.ToDisplayString();

        // Get all properties with PbfMember attribute
        var properties = new List<PbfMemberProperty>();
        
        foreach (var member in classSymbol.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                var pbfMemberAttribute = property.GetAttributes()
                    .FirstOrDefault(attr => attr.AttributeClass?.Name == "PbfMemberAttribute" || attr.AttributeClass?.Name == "PbfMember");

                if (pbfMemberAttribute != null)
                {
                    // PbfMemberAttribute must have 1 constructor argument - field number
                    if (pbfMemberAttribute.ConstructorArguments.Length != 1)
                    {
                        continue;
                    }

                    var fieldNumberValue = pbfMemberAttribute.ConstructorArguments[0].Value;
                    if (fieldNumberValue is not int fieldNumber)
                    {
                        continue;
                    }

                    var attributeValues = new Dictionary<string, object?>();
                    
                    // Extract named arguments from the attribute
                    foreach (var namedArgument in pbfMemberAttribute.NamedArguments)
                    {
                        attributeValues[namedArgument.Key] = namedArgument.Value.Value;
                    }
                    
                    // Extract constructor arguments from the attribute
                    for (int i = 0; i < pbfMemberAttribute.ConstructorArguments.Length; i++)
                    {
                        var constructorArg = pbfMemberAttribute.ConstructorArguments[i];
                        var parameterName = pbfMemberAttribute.AttributeConstructor?.Parameters[i].Name ?? $"Arg{i}";
                        attributeValues[parameterName] = constructorArg.Value;
                    }
                    
                    properties.Add(new PbfMemberProperty(
                        property.Name,
                        property.Type.ToDisplayString(),
                        fieldNumber));
                }
            }
        }

        return new PbfMessageSerializer(className, classNamespace, properties);
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

public record class PbfMessageSerializer
{
    public string TypeName { get; private set; }

    public string Namespace { get; private set; }
    
    public List<PbfMemberProperty> Properties { get; private set; }

    public PbfMessageSerializer(string typeName, string @namespace, List<PbfMemberProperty> properties)
    {
        TypeName = typeName;
        Namespace = @namespace;
        Properties = properties;
    }
}

public record class PbfMemberProperty
{
    public string Name { get; set; }
    public string TypeName { get; set; }
    public int FieldNumber { get; set; }
    
    public PbfMemberProperty(string name, string typeName, int fieldNumber)
    {
        Name = name;
        TypeName = typeName;
        FieldNumber = fieldNumber;
    }
}
