using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace PbfLite.Generator;
internal class SerializerBuilder
{
    private readonly PbfMessageSerializer _serializer;
    private readonly StringBuilder _builder = new();
    private int _indentLevel = 0;

    public SerializerBuilder(PbfMessageSerializer serializer)
    {
        _serializer = serializer;
    }

    public SerializerBuilderResult Build()
    {
        WritePrologue();
        WriteNamespace(
            () => WriteClass(
                () =>
                {
                    WriteDeserializeMethod();
                })
        );

        return new SerializerBuilderResult
        {
            FileName = $"{_serializer.TypeName}.PbfLite.g.cs",
            SourceCode = SourceText.From(_builder.ToString(), Encoding.UTF8).ToString()
        };
    }

    private void WritePrologue()
    {
        _builder.AppendLine("#nullable enable");
        _builder.AppendLine();
        _builder.AppendLine("using PbfLite;");
        _builder.AppendLine();
    }

    private void WriteNamespace(Action content)
    {
        WriteIntended("namespace ").AppendLine(_serializer.Namespace);
        WriteBlock(content);
    }

    private void WriteClass(Action content)
    {
        WriteIntended("public partial class ").AppendLine(_serializer.TypeName);
        WriteBlock(content);
    }

    private void WriteDeserializeMethod()
    {
        WriteIntendedLine($"public static {_serializer.TypeName} Deserialize(PbfBlock pbf)");
        WriteBlock(() =>
        {
            WriteIntended("var result = new ").Append(_serializer.TypeName).AppendLine("();");
            WriteLine();
            WriteIntended("var (fieldNumber, wireType) = pbf.ReadFieldHeader();").AppendLine();
            WriteIntended("while (fieldNumber != 0)").AppendLine();

            WriteBlock(() =>
            {
                WriteIntended("switch (fieldNumber)").AppendLine();

                WriteBlock(() =>
                {
                    //foreach (var field in _serializer.Fields)
                    //{
                    //    WriteIntended($"case {field.FieldNumber}: result.{field.PropertyName} = pbf.Read{field.Type}();").AppendLine();
                    //}
                    WriteSwitchCase("default", () =>
                    {
                        WriteIntendedLine("pbf.SkipField(wireType);");
                        WriteIntendedLine("break;");
                    });
                });

                WriteLine();
                WriteIntended("(fieldNumber, wireType) = pbf.ReadFieldHeader();").AppendLine();
            });

            WriteLine();
            WriteIntendedLine("return result;");
        });
    }

    private void WriteBlock(Action content)
    {
        WriteIntendedLine("{");
        _indentLevel++;

        content.Invoke();

        _indentLevel--;
        WriteIntendedLine("}");
    }

    private void WriteSwitchCase(string @case, Action content)
    {
        if (@case == "default")
        {
            WriteIntended("default:").AppendLine();
        }
        else
        {
            WriteIntended("case ").Append(@case).Append(":").AppendLine();
        }
        _indentLevel++;

        content.Invoke();

        _indentLevel--;
    }

    private StringBuilder WriteLine()
    {
        _builder.AppendLine();
        return _builder;
    }

    private StringBuilder WriteIntended(string value)
    {
        _builder.Append(new string(' ', _indentLevel * 4)).Append(value);
        return _builder;
    }

    private StringBuilder WriteIntendedLine(string line)
    {
        _builder.Append(new string(' ', _indentLevel * 4)).AppendLine(line);
        return _builder;
    }
}

internal class SerializerBuilderResult
{
    public required string FileName { get; set; }

    public required string SourceCode { get; set; }
}