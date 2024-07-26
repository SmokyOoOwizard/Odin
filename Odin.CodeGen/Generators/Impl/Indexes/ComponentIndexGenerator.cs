﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.Abstractions.Components;
using Odin.CodeGen.Abstractions;

namespace Odin.Component.CodeGen.Generators.Impl.Indexes;

[Generator]
public class ComponentIndexGenerator : AComponentIncrementalGenerator
{
    private static readonly string IndexByFullName = typeof(IndexByAttribute).FullName!;

    protected override void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<INamedTypeSymbol> components
    )
    {
        var indexedComponents = components.Where(c =>
        {
            var attributes = c.GetAttributes();

            var hasAttribute = attributes.Any(a => a.AttributeClass!.ToDisplayString() == IndexByFullName);

            var hasMembers = c.GetMembers().Any(w => w is IFieldSymbol);

            return hasAttribute && hasMembers;
        });

        foreach (var indexedComponent in indexedComponents)
        {
            GenerateIndexDefinitions(context, indexedComponent);
        }
    }

    private string GetComponentName(INamedTypeSymbol symbol)
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

    private void GenerateIndexDefinitions(
        GeneratorExecutionContext context,
        INamedTypeSymbol component
    )
    {
        var namespaceName = component.ContainingNamespace.ToDisplayString();
        var componentName = GetComponentName(component);
        var indexName = $"{componentName}Index";

        var path = namespaceName.Replace('.', '/');
        var fullPath = $"{path}/{indexName}";

        var @params = component
                     .GetMembers()
                     .Where(c => c is IFieldSymbol)
                     .Select(m =>
                      {
                          var field = (m as IFieldSymbol)!;
                          return $"{field.Type.ToDisplayString()} {field.Name}";
                      })
                     .ToArray();

        var indexCode = $@"
// <auto-generated/>

using Odin.Abstractions.Entities.Indexes;
using Odin.Abstractions.Entities;

namespace {namespaceName};

public abstract class A{indexName} : AComponentIndex
{{
}}

public sealed class {indexName} : A{indexName}
{{
    public IEntitiesCollection GetEntities()
    {{
        throw new System.NotImplementedException();
    }}

    public IEntitiesCollection GetEntities({string.Join(", ", @params)})
    {{
        throw new System.NotImplementedException();
    }}
}}
";

        context.AddSource($"{fullPath}.g.cs", SourceText.From(indexCode, Encoding.UTF8));

        var extensionName = $"{componentName}IndexExtensions";
        var extensionFullPath = $"{path}/{extensionName}";

        var extensionCode = $@"
// <auto-generated/>

using Odin.Abstractions.Entities.Indexes;
using Odin.Abstractions.Entities;
using Odin.Abstractions.Contexts;

namespace {namespaceName};

public static class {extensionName}
{{
    public static {indexName} Index<T>(this AEntityContext context) where T : A{indexName}
    {{
        if (typeof(T) != typeof({indexName}))
        {{
            throw new System.ArgumentException(nameof(T));
        }}

        throw new System.NotImplementedException();
    }}
}}
";

        context.AddSource($"{extensionFullPath}.g.cs", SourceText.From(extensionCode, Encoding.UTF8));
    }
}