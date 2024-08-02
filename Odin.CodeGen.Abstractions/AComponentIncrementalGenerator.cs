using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odin.Core.Abstractions.Components;

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
        var components = context.Compilation.SyntaxTrees
                       .SelectMany(st => st
                                        .GetRoot()
                                        .DescendantNodes()
                                        .OfType<StructDeclarationSyntax>())
                       .Select(s =>
                        {
                            var sm = context.Compilation.GetSemanticModel(s.SyntaxTree);

                            return sm.GetDeclaredSymbol(s)!;
                        })
                       .OfType<INamedTypeSymbol>()
                       .Where(s =>
                                  s.AllInterfaces.Any(i => i.OriginalDefinition.ToDisplayString() ==
                                                           ComponentInterfaceName));

        GenerateCode(context, components);
    }
}