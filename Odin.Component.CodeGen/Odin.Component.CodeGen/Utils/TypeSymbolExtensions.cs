using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components.Declaration;

namespace Odin.Component.CodeGen.Utils;

public static class TypeSymbolExtensions
{
    public static EFieldType? GetFieldType(this ITypeSymbol type)
    {
        EFieldType? fieldType = type.Name switch
        {
            "Int8" => EFieldType.Int8,
            "Int16" => EFieldType.Int16,
            "Int32" => EFieldType.Int32,
            "Int64" => EFieldType.Int64,
            "UInt8" => EFieldType.UInt8,
            "UInt16" => EFieldType.UInt16,
            "UInt32" => EFieldType.UInt32,
            "UInt64" => EFieldType.UInt64,
            "Single" => EFieldType.Float,
            "Double" => EFieldType.Double,
            "Boolean" => EFieldType.Bool,
            "String" => EFieldType.String,
            _ => null
        };
        return fieldType;
    }
}