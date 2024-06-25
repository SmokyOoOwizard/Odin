using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components;

namespace Odin.CodeGen.Abstractions;

public abstract class AComponentIncrementalGenerator : ISourceGenerator
{
    private static readonly string ComponentInterfaceName = typeof(IComponent).FullName!;

    protected abstract void GenerateCode(GeneratorExecutionContext context, IEnumerable<INamedTypeSymbol> components);

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var components = GetAllTypes(context.Compilation)
           .Where(c =>
            {
                var interfaces = c?.Interfaces;
                if (interfaces == null)
                    return false;

                // Go through all attributes of the class.
                foreach (var baseType in interfaces)
                {
                    var interfaceName = baseType.OriginalDefinition.ToDisplayString();

                    if (interfaceName != ComponentInterfaceName)
                        continue;

                    return true;
                }

                return false;
            });
        
        GenerateCode(context, components);
    }


    IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation) =>
        GetAllTypes(compilation.GlobalNamespace);

    IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
    {
        foreach (var type in @namespace.GetTypeMembers())
            foreach (var nestedType in GetNestedTypes(type))
                yield return nestedType;

        foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
            foreach (var type in GetAllTypes(nestedNamespace))
                yield return type;
    }

    IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
    {
        yield return type;
        foreach (var nestedType in type.GetTypeMembers()
                                       .SelectMany(nestedType => GetNestedTypes(nestedType)))
            yield return nestedType;
    }
}