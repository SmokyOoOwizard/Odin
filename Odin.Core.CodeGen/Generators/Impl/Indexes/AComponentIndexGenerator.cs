using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Declaration;
using Odin.CodeGen.Abstractions;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.Core.CodeGen.Generators.Impl.Indexes;

public abstract class AComponentIndexGenerator : AComponentIncrementalGenerator
{
    private static readonly string IndexByFullName = typeof(IndexByAttribute).FullName!;

    
    protected override void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<INamedTypeSymbol> components
    )
    {
        var indexedComponents = components
                               .Where(c =>
                                {
                                    var attributes = c.GetAttributes();

                                    var hasAttribute = attributes.Any(a => a.AttributeClass!.ToDisplayString() == IndexByFullName);

                                    var hasMembers = c.GetMembers().Any(w => w is IFieldSymbol);

                                    return hasAttribute && hasMembers;
                                })
                               .Select(c =>
                                {
                                    var indexes = ComponentFieldProcessor.GetFieldDeclarations(c.GetMembers());
                                    return new
                                    {
                                        Component = c, Indexes = indexes
                                    };
                                }).Where(c => c.Indexes.Any());

        foreach (var tuple in indexedComponents)
        {
            GenerateIndexDefinitions(context, tuple.Component, tuple.Indexes);
        }
    }

    protected abstract void GenerateIndexDefinitions(
        GeneratorExecutionContext context,
        INamedTypeSymbol component,
        IEnumerable<ComponentFieldDeclaration> componentFieldDeclarations
    );
}