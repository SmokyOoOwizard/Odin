using Microsoft.Data.Sqlite;
using Odin.Core.Components.Declaration;
using Odin.Utils;

namespace Odin.Db.Sqlite.Utils;

public class SqliteComponentDeleter : ISqliteComponentDeleter
{
    public void Delete(
        SqliteConnection connection,
        ulong entityId,
        ulong contextId,
        ulong componentTypeId,
        bool old = false
    )
    {
        if (!ComponentDeclarations.TryGet(componentTypeId, out var componentDeclaration))
            throw new Exception("Unknown component type");

        var tableName = (old ? "__old_" : "") + componentDeclaration.Name.Replace('.', '_');

        var sql = $@"DELETE FROM {tableName} WHERE entityId = {entityId} AND contextId = {contextId.MapToLong()};";

        using var command = connection.CreateCommand();

        command.CommandText = sql;
        command.ExecuteNonQuery();
    }
}