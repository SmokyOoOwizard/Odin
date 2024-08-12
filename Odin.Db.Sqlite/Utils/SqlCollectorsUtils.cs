using Microsoft.Data.Sqlite;
using Odin.Utils;

namespace Odin.Db.Sqlite.Utils;

internal static class SqlCollectorsUtils
{
    public static string GetTableName(ulong contextId, string name)
    {
        var tableName = $"collector_{contextId}_{name}";

        return tableName;
    }

    public static long IncreaseCollectorGeneration(SqliteConnection connection, ulong contextId, string name)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"UPDATE collectors SET generation = generation + 1 WHERE contextId = {contextId.MapToLong()} AND name = '{name}' RETURNING generation;";

        using var reader = command.ExecuteReader();

        if (!reader.Read())
            throw new Exception(); // TODO

        return reader.GetInt64(0);
    }
    
    public static IEnumerable<ulong> GetEntitiesFromCollector(SqliteConnection connection, ulong contextId, string name)
    {
        if (!CollectorExists(connection, contextId, name))
            yield break;

        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);
        command.CommandText = $"SELECT entityId FROM {tableName};";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var entityId = reader.GetInt64(0).MapToUlong();
            yield return entityId;
        }
    }

    public static void AddEntityToCollector(SqliteConnection connection, ulong contextId, string name, ulong entityId)
    {
        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);

        command.CommandText =
            $"INSERT INTO {tableName} (entityId, contextId, generation) " +
            $"SELECT {entityId}, {contextId}, collectors.generation FROM collectors " +
            $"WHERE contextId = {contextId.MapToLong()} AND name = '{name}';";

        command.ExecuteNonQuery();
    }

    public static void ClearCollector(SqliteConnection connection, ulong contextId, string name)
    {
        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);

        command.CommandText = $"DELETE FROM {tableName};";
        command.ExecuteNonQuery();
    }

    public static bool CollectorExists(SqliteConnection connection, ulong contextId, string name)
    {
        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);

        // todo. wtf? check collectors table for row
        command.CommandText = $"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}';";
        using var reader = command.ExecuteReader();

        return reader.Read();
    }

    public static IEnumerator<CollectorInfo> GetCollectors(SqliteConnection connection, ulong contextId)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT name, matcherId FROM collectors WHERE contextId = {contextId.MapToLong()};";

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var name = reader.GetString(0);
            var matcherId = reader.GetInt64(1).MapToUlong();
            var generation = reader.GetInt64(2);
            yield return new CollectorInfo
            {
                ContextId = contextId,
                Name = name,
                MatcherId = matcherId,
                Generation = generation
            };
        }
    }

    public static void CreateCollector(SqliteConnection connection, ulong contextId, string name, ulong matcherId)
    {
        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);

        command.CommandText =
            $"INSERT INTO collectors (contextId, name, matcherId, generation) VALUES ({contextId.MapToLong()}, '{name}', {matcherId.MapToLong()}, 0);" +
            $"CREATE TABLE IF NOT EXISTS {tableName} (id INTEGER PRIMARY KEY, entityId INTEGER, contextId INTEGER, generation INTEGER NOT NULL, CONSTRAINT entity UNIQUE (entityId, contextId, generation));";

        command.ExecuteNonQuery();
    }

    public static void DeleteCollector(SqliteConnection connection, ulong contextId, string name)
    {
        using var command = connection.CreateCommand();
        var tableName = GetTableName(contextId, name);

        command.CommandText =
            $"DELETE FROM collectors WHERE contextId = {contextId.MapToLong()} AND name = '{name}';" +
            $"DROP TABLE IF EXISTS {tableName};";

        command.ExecuteNonQuery();
    }
}