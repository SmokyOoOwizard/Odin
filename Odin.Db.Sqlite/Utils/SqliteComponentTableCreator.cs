using System.Text;
using Microsoft.Data.Sqlite;
using Odin.Core.Abstractions.Components.Declarations;
using Odin.Core.Components.Declaration;

namespace Odin.Db.Sqlite.Utils;

public class SqliteComponentTableCreator : ISqliteComponentTableCreator
{
    public void CreateTables(SqliteConnection connection)
    {
        var components = ComponentDeclarations.GetComponentDeclarations();

        var sb = new StringBuilder();

        var fieldsSb = new StringBuilder();

        foreach (var declaration in components)
        {
            var tableName = declaration.Name.Replace('.', '_');
            var componentName = declaration.Name;
            var componentId = declaration.Id;

            fieldsSb.Clear();

            foreach (var fieldDeclaration in declaration.Fields)
            {
                fieldsSb.Append($", {fieldDeclaration.Name}");

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

                fieldsSb.Append(" ");
                fieldsSb.Append(type);

                var collection = fieldDeclaration.CollectionType switch
                {
                    ECollectionType.None => "",
                    ECollectionType.Array => "[]",
                    _ => throw new ArgumentOutOfRangeException()
                };

                fieldsSb.Append(collection);
            }

            var fields = fieldsSb.ToString();

            sb.Append(
                $"CREATE TABLE IF NOT EXISTS {tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, entityId INTEGER, contextId INTEGER{fields}, CONSTRAINT entity UNIQUE (entityId, contextId));");
            sb.Append(
                $"CREATE TABLE IF NOT EXISTS __old_{tableName} (id INTEGER PRIMARY KEY AUTOINCREMENT, entityId INTEGER, contextId INTEGER{fields}, CONSTRAINT entity UNIQUE (entityId, contextId));");
            sb.Append(
                $"INSERT OR IGNORE INTO componentTypes (name, type, tableName) VALUES ('{componentName}', {componentId}, '{tableName}');");
        }

        using var command = connection.CreateCommand();

        command.CommandText = sb.ToString();
        command.ExecuteNonQuery();
    }
}