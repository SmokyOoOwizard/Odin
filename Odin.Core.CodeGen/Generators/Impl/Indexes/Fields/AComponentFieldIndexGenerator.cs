using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Odin.CodeGen.Abstractions;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.CodeGen.Generators.Impl.Indexes.Fields;

public abstract class AComponentFieldIndexGenerator : AComponentIncrementalGenerator
{
    protected override void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<INamedTypeSymbol> components
    )
    {
        var indexedComponents = components
                               .Select(c =>
                                {
                                    var indexes = ComponentFieldProcessor.GetFieldDeclarations(c.GetMembers())
                                                                         .Where(a => a.IsIndex);
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