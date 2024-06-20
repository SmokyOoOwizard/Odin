using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Generators;

public abstract class AComponentIncrementalGenerator : IIncrementalGenerator
{
    private static readonly string ComponentInterfaceName = typeof(IComponent).FullName!;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
                              .CreateSyntaxProvider(
                                   (s, _) => s is StructDeclarationSyntax,
                                   (ctx, _) => GetClassDeclarationForSourceGen(ctx))
                              .Where(t => t.reportAttributeFound)
                              .Select((t, _) => t.Item1);

        context.RegisterSourceOutput(context.CompilationProvider.Combine(provider.Collect()),
                                     (ctx, t) => GenerateCode(ctx, t.Left, t.Right));
    }

    private static (StructDeclarationSyntax, bool reportAttributeFound) GetClassDeclarationForSourceGen(
        GeneratorSyntaxContext context
    )
    {
        var structDeclarationSyntax = (StructDeclarationSyntax)context.Node;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(structDeclarationSyntax) as INamedTypeSymbol;

        var interfaces = typeSymbol?.Interfaces;
        if (interfaces == null)
            return (structDeclarationSyntax, false);

        // Go through all attributes of the class.
        foreach (var baseType in interfaces)
        {
            var interfaceName = baseType.OriginalDefinition.ToDisplayString();

            if (interfaceName != ComponentInterfaceName)
                continue;

            return (structDeclarationSyntax, true);
        }

        return (structDeclarationSyntax, false);
    }

    protected abstract void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> structDeclarations
    );
}