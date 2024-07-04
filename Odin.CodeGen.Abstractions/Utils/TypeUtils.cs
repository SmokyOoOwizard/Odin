using Microsoft.CodeAnalysis;

namespace Odin.CodeGen.Abstractions.Utils;

public static class TypeUtils
{
    public static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation) =>
        GetAllTypes(compilation.GlobalNamespace);

    public static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
    {
        foreach (var type in @namespace.GetTypeMembers())
            foreach (var nestedType in GetNestedTypes(type))
                yield return nestedType;

        foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            foreach (var type in GetAllTypes(nestedNamespace))
                yield return type;
    }

    public static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nestedType in type.GetTypeMembers()
                                       .SelectMany(GetNestedTypes))
            yield return nestedType;
    }
}