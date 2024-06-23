﻿using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Odin.Abstractions.Components.Declaration;
using Odin.Component.CodeGen.Utils;

namespace Odin.Component.CodeGen.Generators.Impl;

[Generator]
public class ComponentSerializationGenerator : AComponentIncrementalGenerator
{
    protected override void GenerateCode(
        SourceProductionContext context,
        Compilation compilation,
        ImmutableArray<StructDeclarationSyntax> structDeclarations
    )
    {
        var namespaceName = compilation.AssemblyName;

        foreach (var structDeclarationSyntax in structDeclarations)
        {
            var semanticModel = compilation.GetSemanticModel(structDeclarationSyntax.SyntaxTree);

            if (semanticModel.GetDeclaredSymbol(structDeclarationSyntax) is not INamedTypeSymbol structSymbol)
                continue;

            var componentName = structSymbol.Name;
            var componentFullName = structSymbol.OriginalDefinition.ToDisplayString();

            if (string.IsNullOrWhiteSpace(componentName))
                continue;

            var path = structSymbol.Locations.First().GetLineSpan().Path;

            var fileName = Path.GetFileNameWithoutExtension(path);

            var members = structSymbol.GetMembers();

            var fields = ComponentFieldProcessor.GetFieldDeclarations(members).ToArray();

            var code = $@"
// <auto-generated/>

using System;
using System.Collections.Generic;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Declaration;
using Odin.Abstractions.Serialization;

namespace {namespaceName};

public class {componentName}Serializer : IComponentSerializer<{componentFullName}>
{{
    public SerializedComponent Serialize({componentFullName} component)
    {{
        var componentTypeId = ComponentDeclarations.Instance.GetComponentTypeId<TestComponent>();
        if (!componentTypeId.HasValue)
            throw new Exception(""Component not registered""); // todo

        var serializedComponent = new SerializedComponent();
        serializedComponent.Id = componentTypeId.Value;

        serializedComponent.Fields = new SerializedField[{fields.Length}];
        {string.Join("\n", GetFieldsSerialization(fields))}
        return serializedComponent;
    }}

    public SerializedComponent[] Serialize({componentFullName}[] components)
    {{
        var serializedComponents = new SerializedComponent[components.Length];

        for (var i = 0; i < components.Length; i++)
        {{
            serializedComponents[i] = Serialize(components[i]);
        }}
        
        return serializedComponents;
    }}

    public {componentFullName} Deserialize(SerializedComponent serializedComponent)
    {{
         var component = new {componentFullName}();
        
        {string.Join("\n\t\t", GetFieldsDeserialization(fields))}
        
        return component;
    }}

    public {componentFullName}[] Deserialize(SerializedComponent[] serializedComponents)
    {{
        var components = new {componentFullName}[serializedComponents.Length];

        for (var i = 0; i < components.Length; i++)
        {{
            components[i] = Deserialize(serializedComponents[i]);
        }}
        
        return components;
    }}
}}
";

            context.AddSource($"{fileName}Serializer.g.cs", SourceText.From(code, Encoding.UTF8));
        }
    }

    private static IEnumerable<string> GetFieldsSerialization(ComponentFieldDeclaration[] fields)
    {
        var offset = 0;
        
        foreach (var field in fields)
        {
            var result = GetFieldSerialization(field, offset);
            offset++;
            yield return result;
        }
    }

    private static string GetFieldSerialization(ComponentFieldDeclaration declaration, int offset)
    {
        var result = $@"
        serializedComponent.Fields[{offset}] = new SerializedField()
        {{
            Name = ""{declaration.Name}"",
            CollectionType = ECollectionType.{declaration.CollectionType},
            IsIndex = {declaration.IsIndex.ToString().ToLower()},
            Type = EFieldType.{declaration.Type},
            Value = component.{declaration.Name},
        }};
";

        return result;
    }
    
    private static IEnumerable<string> GetFieldsDeserialization(ComponentFieldDeclaration[] fields)
    {
        var offset = 0;
        
        foreach (var field in fields)
        {
            var result = GetFieldDeserialization(field, offset);
            offset++;
            yield return result;
        }
    }
    
    private static string GetFieldDeserialization(ComponentFieldDeclaration declaration, int offset)
    {
        var castType = declaration.Type.GetFieldType() + (declaration.CollectionType == ECollectionType.Array ? "[]" : "");
        
        var result = $"component.{declaration.Name} = ({castType})(serializedComponent.Fields[{offset}].Value)!;";

        return result;
    }
}