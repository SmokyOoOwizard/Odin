﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;
using Odin.Utils;

namespace Odin.Core.CodeGen.Generators.Impl.Indexes.Fields;

[Generator]
public class InMemoryComponentFieldIndexImplGenerator : AComponentFieldIndexGenerator
{
    protected override void GenerateIndexDefinitions(
        GeneratorExecutionContext context,
        INamedTypeSymbol component,
        IEnumerable<ComponentFieldDeclaration> componentFieldDeclarations
    )
    {
        var namespaceName = component.ContainingNamespace.ToDisplayString();
        var fullName = component.OriginalDefinition.ToDisplayString();
        var componentName = ComponentUtils.GetComponentName(component);
        var indexModuleName = $"InMemory{componentName}IndexModule";
        var indexName = $"I{componentName}Index";

        var indexes = componentFieldDeclarations
                     .Where(c => c.IsIndex)
                     .ToArray();

        var storages = GenerateIndexesStorage(indexes);

        var componentId = TypeComponentUtils.GetComponentTypeId(fullName);

        var path = namespaceName.Replace('.', '/');
        var fullPath = $"{path}/{indexModuleName}";

        var indexCode = $@"
// <auto-generated/>

using Odin.Core.Entities;
using Odin.Core.Components;
using Odin.Core.Repositories.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Entities.Collections.Impl;
using Odin.Core.Indexes;
using Odin.Utils;
using System.Collections.Generic;
using System.Linq;
using System;

namespace {namespaceName};

public sealed class {indexModuleName} : {indexName}, IInMemoryIndexModule
{{
{storages}
    private readonly Dictionary<EntityId, {fullName}> _idsToComponent = new();

    private IReadOnlyEntityRepository _components;
    private IEntityComponentsRepository _changes;

    public void SetRepositories(IReadOnlyEntityRepository components, IEntityComponentsRepository changes)
    {{
        _components = components;
        _changes = changes;
    }}

    public ulong GetComponentTypeId()
    {{
        return {componentId}ul;
    }}

    public void Add(ComponentWrapper component, EntityId id)
    {{
        if (component.TypeId != {componentId})
            return;

        var realComponent = ({fullName})component.Component;

        {GenerateAddIndexCode(indexes)}
        
        _idsToComponent[id] = realComponent;
    }}

    public void Remove(EntityId id)
    {{
        if (_idsToComponent.TryGetValue(id, out var component))
        {{{GenerateRemoveIndexCode(indexes)}
          
            _idsToComponent.Remove(id);
        }}
    }}

    public IEntitiesCollection GetEntities()
    {{
        var entities = _idsToComponent.Keys.Select(id => new Entity(id, _components, _changes)).ToArray();

        return new InMemoryEntitiesCollection(entities);
    }}

{GenerateIndexesAccess(indexes)}
}}
";

        context.AddSource($"{fullPath}.g.cs", SourceText.From(indexCode, Encoding.UTF8));
    }

    private string GenerateIndexesAccess(IEnumerable<ComponentFieldDeclaration> indexes)
    {
        var code = indexes.Select(c =>
        {
            if (c.CollectionType == ECollectionType.None)
                return GenerateNonCollectionIndexesAccess(c);

            return GenerateCollectionIndexesAccess(c);
        });

        return string.Join("", code);
    }

    private string GenerateNonCollectionIndexesAccess(ComponentFieldDeclaration index)
    {
        return $@"
    public IEntitiesCollection {index.Name}({index.GetFieldType()} value)
    {{
        if (!_{index.Name}ToIds.TryGetValue(value, out var {index.Name}Ids))
            return new InMemoryEntitiesCollection();

        var ids = {index.Name}Ids.Select(id => new Entity(id, _components, _changes)).ToArray();

        return new InMemoryEntitiesCollection(ids);
    }}
";
    }

    private string GenerateCollectionIndexesAccess(ComponentFieldDeclaration index)
    {
        return $@"
    public IEntitiesCollection {index.Name}(params {index.GetFieldType()} values)
    {{
        if (!_{index.Name}ToIds.TryGetValue(values, out var {index.Name}Ids))
            return new InMemoryEntitiesCollection();

        var ids = {index.Name}Ids.Select(id => new Entity(id, _components, _changes)).ToArray();

        return new InMemoryEntitiesCollection(ids);
    }}

    public IEntitiesCollection {index.Name}Contains(params {index.GetFieldType()} values)
    {{
        var toReturn = new HashSet<EntityId>();

        foreach (var ids in _{index.Name}ToIds)
        {{
            if (ids.Key.IsSubArray(values))
                toReturn.UnionWith(ids.Value);
        }}

        var returnEntities = toReturn.Select(id => new Entity(id, _components, _changes)).ToArray();

        return new InMemoryEntitiesCollection(returnEntities); 
    }}

    public IEntitiesCollection {index.Name}ContainsAny(params {index.GetFieldType()} values)
    {{
        var toReturn = new HashSet<EntityId>();

        foreach (var ids in _{index.Name}ToIds)
        {{
            foreach (var value in values)
            {{
                if (Array.IndexOf(ids.Key, value) == -1)
                    continue;
                
                toReturn.UnionWith(ids.Value);
                break;
            }}
        }}

        var returnEntities = toReturn.Select(id => new Entity(id, _components, _changes)).ToArray();

        return new InMemoryEntitiesCollection(returnEntities);
    }}
";
    }

    private string GenerateAddIndexCode(IEnumerable<ComponentFieldDeclaration> indexes)
    {
        var indexesInfo = indexes.Select(c => new
        {
            Type = c.GetFieldType(),
            c.Name
        }).ToArray();

        var removeOldDefinitions = indexesInfo.Select(c => $@"
            var old{c.Name}Value = oldComponent.{c.Name};
            if (realComponent.{c.Name} != old{c.Name}Value)
            {{
                if (_{c.Name}ToIds.TryGetValue(old{c.Name}Value, out var old{c.Name}Ids))
                {{
                    old{c.Name}Ids.Remove(id);
                    if (old{c.Name}Ids.Count == 0)
                        _{c.Name}ToIds.Remove(old{c.Name}Value);
                }}
            }}");

        var addDefinitions = indexesInfo.Select(c => $@"
        var {c.Name}Value = realComponent.{c.Name};
        if (!_{c.Name}ToIds.TryGetValue({c.Name}Value, out var {c.Name}Ids))
            _{c.Name}ToIds[{c.Name}Value] = {c.Name}Ids = new();

        {c.Name}Ids.Add(id);");

        return $@"
        if (_idsToComponent.TryGetValue(id, out var oldComponent))
        {{{string.Join("\n", removeOldDefinitions)}
        }}
{string.Join("\n", addDefinitions)}";
    }

    private string GenerateRemoveIndexCode(IEnumerable<ComponentFieldDeclaration> indexes)
    {
        var removeDefinitions = indexes.Select(c => new
        {
            Type = c.GetFieldType(),
            c.Name
        }).Select(c => $@"
            var {c.Name}Value = component.{c.Name};
            if (_{c.Name}ToIds.TryGetValue({c.Name}Value, out var {c.Name}Ids))
            {{
                {c.Name}Ids.Remove(id);
                if ({c.Name}Ids.Count == 0)
                    _{c.Name}ToIds.Remove({c.Name}Value);
            }}
");


        return $@"{string.Join("", removeDefinitions)}";
    }

    private string GenerateIndexesStorage(IEnumerable<ComponentFieldDeclaration> indexes)
    {
        var storageDefinitions = indexes.Select(c => new
        {
            Type = c.GetFieldType(),
            Collection = c.CollectionType,
            RawType = c.GetFieldType(false),
            c.Name
        }).Select(c =>
        {
            if (c.Collection == ECollectionType.None)
                return $@"
    private readonly Dictionary<{c.Type}, HashSet<EntityId>> _{c.Name}ToIds = new();";
            else
                return $@"
    private readonly Dictionary<{c.Type}, HashSet<EntityId>> _{c.Name}ToIds = new(new ArrayEqualityComparer<{c.RawType}>());";
        });


        return $@"{string.Join("", storageDefinitions)}";
    }
}