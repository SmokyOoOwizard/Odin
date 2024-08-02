﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Core.CodeGen.Generators.Impl.Indexes.Fields;

[Generator]
public class ComponentFieldIndexInterfaceGenerator : AComponentFieldIndexGenerator
{
    protected override void GenerateIndexDefinitions(
        GeneratorExecutionContext context,
        INamedTypeSymbol component,
        IEnumerable<ComponentFieldDeclaration> componentFieldDeclarations
    )
    {
        var namespaceName = component.ContainingNamespace.ToDisplayString();
        var componentName = ComponentUtils.GetComponentName(component);
        var indexName = $"{componentName}Index";


        var path = namespaceName.Replace('.', '/');
        var fullPath = $"{path}/I{indexName}";
        
        var indexes = componentFieldDeclarations
           .Select(c =>
            {
                var name = c.Name;
                var type = c.GetFieldType();

                if (c.CollectionType != ECollectionType.None)
                {
                    return $@"
    IEntitiesCollection {name}(params {type} values);

    IEntitiesCollection {name}Contains(params {type} values);

    IEntitiesCollection {name}ContainsAny(params {type} values);
";
                }

                return $@"
    IEntitiesCollection {name}({type} value);
";
            });
        var indexCode = $@"
// <auto-generated/>

using System;
using Odin.Core.Indexes;
using Odin.Core.Entities.Collections;

namespace {namespaceName};

public interface I{indexName} : IIndexModule
{{
    {string.Join("\n\t\t\t", indexes)}
}}
";

        context.AddSource($"{fullPath}.g.cs",
                          SourceText.From(indexCode, Encoding.UTF8));
    }
}