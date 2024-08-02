using Microsoft.CodeAnalysis;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.CodeGen.Abstractions.Utils;

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
    
    public static string GetFieldType(this ComponentFieldDeclaration field, bool useCollection = true)
    {
        var fieldType = field.Type switch
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

        if (useCollection && field.CollectionType != ECollectionType.None)
        {
            fieldType = field.CollectionType switch
            {
                ECollectionType.Array => $"{fieldType}[]",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        return fieldType;
    }
    
    public static TypedConstantKind GetTypedConstantKind(this ITypeSymbol type)
    {
        switch (type.SpecialType)
        {
            case SpecialType.System_Boolean:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_Byte:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Char:
            case SpecialType.System_String:
            case SpecialType.System_Object:
                return TypedConstantKind.Primitive;
            default:
                switch (type.TypeKind)
                {
                    case TypeKind.Array:
                        return TypedConstantKind.Array;
                    case TypeKind.Enum:
                        return TypedConstantKind.Enum;
                    case TypeKind.Error:
                        return TypedConstantKind.Error;
                }

                if (type.IsReferenceType)
                {
                    return TypedConstantKind.Type;
                }

                return TypedConstantKind.Error;
        }
    }
}