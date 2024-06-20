using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components.Declaration;

namespace Odin.Component.CodeGen.Utils;

public static class TypeSymbolExtensions
{
    public static FieldType? GetFieldType(this ITypeSymbol type)
    {
        FieldType? fieldType = type.Name switch
        {
            "Int8" => FieldType.Int8,
            "Int16" => FieldType.Int16,
            "Int32" => FieldType.Int32,
            "Int64" => FieldType.Int64,
            "UInt8" => FieldType.UInt8,
            "UInt16" => FieldType.UInt16,
            "UInt32" => FieldType.UInt32,
            "UInt64" => FieldType.UInt64,
            "Single" => FieldType.Float,
            "Double" => FieldType.Double,
            "Boolean" => FieldType.Bool,
            "String" => FieldType.String,
            _ => null
        };
        return fieldType;
    }
}