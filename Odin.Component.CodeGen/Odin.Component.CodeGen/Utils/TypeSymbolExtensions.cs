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
    
    public static string GetFieldType(this EFieldType type)
    {
        var fieldType = type switch
        {
            EFieldType.Int8 => "Int8",
            EFieldType.Int16 => "Int16",
            EFieldType.Int32 => "Int32",
            EFieldType.Int64 => "Int64",
            EFieldType.UInt8 => "UInt8",
            EFieldType.UInt16 => "UInt16",
            EFieldType.UInt32 => "UInt32",
            EFieldType.UInt64 => "UInt64",
            EFieldType.Float => "Single",
            EFieldType.Double => "Double",
            EFieldType.Bool => "Boolean",
            EFieldType.String => "String",
            _ => string.Empty
        };
        return fieldType;
    }
}