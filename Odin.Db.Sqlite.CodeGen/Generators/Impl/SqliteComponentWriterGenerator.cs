﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.Abstractions.Components.Utils;
using Odin.CodeGen.Abstractions;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Db.Sqlite.CodeGen.Generators.Impl;

[Generator]
public class SqliteComponentWriterGenerator : AComponentIncrementalGenerator
{
    protected override void GenerateCode(
        GeneratorExecutionContext context,
        IEnumerable<INamedTypeSymbol> components
    )
    {
        var namespaceName = context.Compilation.AssemblyName;

        var tablesSql = components
                       .Select(c =>
                        {
                            var componentName = c.OriginalDefinition.ToDisplayString();

                            var tableName = componentName.Replace('.', '_');

                            var members = c.GetMembers();
                            var fieldsDeclarations = ComponentFieldProcessor.GetFieldDeclarations(members).ToArray();
                            var fields = fieldsDeclarations
                                        .Select(fieldDeclaration => fieldDeclaration.Name)
                                        .ToArray();

                            var additionalFieldsName = fields.Any() ? $", {string.Join(", ", fields)}" : string.Empty;
                            var additionalFields = fields.Any()
                                ? $", {string.Join(", ", fieldsDeclarations.Select(f => {
                                    var mapCode = f.Type switch
                                    {
                                        EFieldType.UInt8 => $"'{{realComponent.{f.Name}.MapToByte()}}'",
                                        EFieldType.UInt16 => $"'{{realComponent.{f.Name}.MapToShort()}}'",
                                        EFieldType.UInt32 => $"'{{realComponent.{f.Name}.MapToInt()}}'",
                                        EFieldType.UInt64 => $"'{{realComponent.{f.Name}.MapToLong()}}'",
                                        _ => $"'{{realComponent.{f.Name}}}'"
                                    };

                                    return mapCode;
                                }))}"
                                : string.Empty;

                            var componentId = TypeComponentUtils.GetComponentTypeId(componentName);

                            var insertSql =
                                $@"
                    case {componentId}:
                        {{
                            var realComponent = ({componentName})component.Component;
                            
                            if (old)
                                sql = $@""INSERT OR REPLACE INTO __old_{tableName} (entityId, contextId{additionalFieldsName}) VALUES ({{entityId}}, {{contextId.MapToLong()}}{additionalFields});"";
                            else
                                sql = $@""INSERT OR REPLACE INTO {tableName} (entityId, contextId{additionalFieldsName}) VALUES ({{entityId}}, {{contextId.MapToLong()}}{additionalFields});"";
                    
                            break;
                        }}
";

                            return insertSql;
                        }).ToArray();

        var insertSwitchCases = string.Join("\n", tablesSql);

        var code = $@"
// <auto-generated/>

using System;
using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;
using Odin.Utils;

namespace {namespaceName};

public class SqliteComponentWriter : ISqliteComponentWriter
{{
    public void Write(SqliteConnection connection, ulong entityId, ulong contextId, ComponentWrapper component, bool old = false)
    {{
        var sql = string.Empty;

        switch (component.TypeId)
        {{
            {insertSwitchCases}
            default:
                throw new Exception(""Unknown component type"");
        }}

        using var command = connection.CreateCommand();

        command.CommandText = sql;
        command.ExecuteNonQuery();
    }}
}}
";

        context.AddSource("SqliteComponentWriter.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}