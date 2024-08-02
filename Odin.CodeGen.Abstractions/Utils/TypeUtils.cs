using Microsoft.CodeAnalysis;

namespace Odin.CodeGen.Abstractions.Utils;

public static class TypeUtils
{
    public static bool CheckBaseType(INamedTypeSymbol symbol, string fullName)
    {
        var baseType = symbol.BaseType;
        while (baseType != null)
        {
            var baseTypeName = baseType.OriginalDefinition.ToDisplayString();
            if (baseTypeName == fullName)
                return true;

            baseType = baseType.BaseType;
        }

        return false;
    }
}