using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components;
using Odin.CodeGen.Abstractions.Utils;

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
        var components = TypeUtils.GetAllTypes(context.Compilation)
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
                                   })
                                  .Where(c => c.DeclaredAccessibility == Accessibility.Public);

        GenerateCode(context, components);
    }
}