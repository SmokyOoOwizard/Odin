﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.CodeGen.Abstractions;
using Odin.CodeGen.Abstractions.Utils;
using Odin.Core.Abstractions.Components.Declarations;

namespace Odin.Db.Sqlite.CodeGen.Generators.Impl;

[Generator]
public class ComponentSqliteTableGenerator : AComponentIncrementalGenerator
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
                var fields = ComponentFieldProcessor.GetFieldDeclarations(members)
                                                    .Select(fieldDeclaration =>
                                                     {
                                                         var name = fieldDeclaration.Name;
                                                         var type = fieldDeclaration.Type switch
                                                         {
                                                             EFieldType.String => "TEXT",
                                                             EFieldType.Int => "INTEGER",
                                                             EFieldType.Int8 => "INTEGER",
                                                             EFieldType.Int16 => "INTEGER",
                                                             EFieldType.Int32 => "INTEGER",
                                                             EFieldType.Int64 => "INTEGER",
                                                             EFieldType.UInt8 => "INTEGER",
                                                             EFieldType.UInt16 => "INTEGER",
                                                             EFieldType.UInt32 => "INTEGER",
                                                             EFieldType.UInt64 => "INTEGER",
                                                             EFieldType.Float => "FLOAT",
                                                             EFieldType.Double => "DOUBLE",
                                                             EFieldType.Bool => "BOOLEAN",
                                                             _ => throw new ArgumentOutOfRangeException()
                                                         };
                                                         return $"{name} {type}" +
                                                                (fieldDeclaration.CollectionType ==
                                                                 ECollectionType.Array
                                                                    ? "[]"
                                                                    : string.Empty);
                                                     }).ToArray();

                var additionalFields = fields.Any() ? $", {string.Join(", ", fields)}" : string.Empty;

                var sql =
                    $"CREATE TABLE IF NOT EXISTS {tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, entityId INTEGER, contextId INTEGER{additionalFields}, CONSTRAINT entity UNIQUE (entityId, contextId));\n\t\t\t"
                  + $"CREATE TABLE IF NOT EXISTS __old_{tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, entityId INTEGER, contextId INTEGER{additionalFields}, CONSTRAINT entity UNIQUE (entityId, contextId));\n\t\t\t"
                  + $"INSERT OR IGNORE INTO componentTypes (name, type, tableName) VALUES ('{componentName}', {{TypeComponentUtils.GetComponentTypeId<{componentName}>()}}, '{tableName}');";

                return sql;
            });

        var sql = string.Join("\n\t\t\t", tablesSql);

        var code = $@"
// <auto-generated/>

using System;
using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite;
using Odin.Abstractions.Components.Utils;

namespace {namespaceName};

public class SqliteComponentTableCreator : ISqliteComponentTableCreator
{{
    public void CreateTables(SqliteConnection connection)
    {{
        using var command = connection.CreateCommand();

        command.CommandText = $@""{sql}"";
        command.ExecuteNonQuery();
    }}
}}
";

        context.AddSource("SqliteComponentTableCreator.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}