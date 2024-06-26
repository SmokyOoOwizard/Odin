using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Odin.Abstractions.Components.Declaration;
using Odin.Abstractions.Components.Utils;
using Odin.CodeGen.Abstractions;
using Odin.CodeGen.Abstractions.Utils;

namespace Odin.Db.Sqlite.CodeGen.Generators.Impl;

[Generator]
public class SqliteComponentReaderGenerator : AComponentIncrementalGenerator
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

                            var componentId = TypeComponentUtils.GetComponentTypeId(componentName);

                            var selectSql = $@"
                    case {componentId}:
                        {{
                            if (old)
                                sql = $@""SELECT entityId{string.Join(", ", additionalFieldsName)} FROM __old_{tableName} WHERE entityId = {{entityId}} AND contextId = {{contextId.MapToLong()}};"";
                            else
                                sql = $@""SELECT entityId{string.Join(", ", additionalFieldsName)} FROM {tableName} WHERE entityId = {{entityId}} AND contextId = {{contextId.MapToLong()}};"";
                            break;
                        }}
";

                            var fillCode = fieldsDeclarations.Select((f, i) =>
                                                              {
                                                                  var realI = i + 1;
                                                                  var mapCode = f.Type switch
                                                                  {
                                                                      EFieldType.String => $"GetString({realI})",
                                                                      EFieldType.Int8 => $"GetByte({realI})",
                                                                      EFieldType.Int16 => $"GetInt16({realI})",
                                                                      EFieldType.Int32 => $"GetInt32({realI})",
                                                                      EFieldType.Int64 => $"GetInt64({realI})",
                                                                      EFieldType.UInt8 =>
                                                                          $"GetByte({realI}).MapToSbyte()",
                                                                      EFieldType.UInt16 =>
                                                                          $"GetInt16({realI}).MapToUshort()",
                                                                      EFieldType.UInt32 =>
                                                                          $"GetInt32({realI}).MapToUint()",
                                                                      EFieldType.UInt64 =>
                                                                          $"GetInt64({realI}).MapToUlong()",
                                                                      EFieldType.Float => $"GetFloat({realI})",
                                                                      EFieldType.Double => $"GetDouble({realI})",
                                                                      EFieldType.Bool => $"GetBoolean({realI})",
                                                                      _ => throw new ArgumentOutOfRangeException()
                                                                  };

                                                                  return $"component.{f.Name} = reader.{mapCode};";
                                                              })
                                                             .ToArray();
                            var fillComponentCode = $@"
                    case {componentId}:
                        {{
                            var component = new {componentName}();
                            {string.Join("\n\t\t\t\t\t\t\t", fillCode)}

                            return new ComponentWrapper({componentId}, component);
                        }}
";

                            return (selectSql, fillComponentCode);
                        }).ToArray();
        
        var selectSwitchCases = string.Join("\n", tablesSql.Select(c => c.selectSql));
        var fillCode = string.Join("\n", tablesSql.Select(c => c.fillComponentCode));

        var code = $@"
// <auto-generated/>

using System;
using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;
using Odin.Db.Sqlite.Utils;

namespace {namespaceName};

public class SqliteComponentReader : ISqliteComponentReader
{{
    public ComponentWrapper Read(SqliteConnection connection, ulong entityId, ulong contextId, ulong componentTypeId, bool old = false)
    {{
        using var command = connection.CreateCommand();

        var sql = string.Empty;

        switch (componentTypeId)
        {{
            {selectSwitchCases}
            default:
                throw new Exception(""Unknown component type"");
        }}

        command.CommandText = sql;
        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return new ComponentWrapper(componentTypeId, null);

        switch (componentTypeId)
        {{
            {fillCode}
            default:
                throw new Exception(""Unknown component type"");
        }}
    }}
}}
";

        context.AddSource("SqliteComponentReader.g.cs", SourceText.From(code, Encoding.UTF8));
    }
}

