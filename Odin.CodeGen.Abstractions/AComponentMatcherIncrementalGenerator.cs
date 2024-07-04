using Microsoft.CodeAnalysis;
using Odin.Abstractions.Collectors.Matcher;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.CodeGen.Abstractions;

public abstract class AComponentMatcherIncrementalGenerator : ISourceGenerator
{
    private static readonly string ComponentMatcherBaseName = typeof(AComponentMatcher).FullName!;

    protected abstract void GenerateCode(GeneratorExecutionContext context, IEnumerable<INamedTypeSymbol> matchers);

    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var components = TypeUtils.GetAllTypes(context.Compilation)
                                  .Where(c =>
                                   {
                                       var baseType = c.BaseType;

                                       var hasMatcherType = false;
                                       while (baseType != null)
                                       {
                                           var baseTypeName = baseType.OriginalDefinition.ToDisplayString();
                                           if (baseTypeName == ComponentMatcherBaseName)
                                           {
                                               hasMatcherType = true;
                                               break;
                                           }

                                           baseType = baseType.BaseType;
                                       }

                                       if (!hasMatcherType)
                                           return false;

                                       return true;
                                   })
                                  .Where(c => c.DeclaredAccessibility == Accessibility.Public);

        GenerateCode(context, components);
    }
}