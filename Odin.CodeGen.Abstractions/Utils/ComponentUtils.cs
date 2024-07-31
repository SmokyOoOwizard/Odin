using Microsoft.CodeAnalysis;

namespace Odin.CodeGen.Abstractions.Utils;

public static class ComponentUtils
{
    public static string GetComponentName(INamedTypeSymbol symbol)
    {
        var name = symbol.Name;

        var parent = symbol.ContainingSymbol;
        while (parent != null && parent is not INamespaceSymbol)
        {
            name = $"{parent.Name}_{name}";
            parent = parent.ContainingSymbol;
        }

        return name;
    }
}