using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite.Utils;

internal static class SqlCommands
{
    private static readonly ISqliteComponentDeleter Deleter = new SqliteComponentDeleter();
    private static readonly ISqliteComponentWriter Writer = new SqliteComponentWriter();
    private static readonly ISqliteComponentReader Reader = new SqliteComponentReader();

    public static ISqliteComponentDeleter GetDeleter()
    {
        return Deleter;
    }

    public static ISqliteComponentWriter GetWriter()
    {
        return Writer;
    }

    public static ISqliteComponentReader GetReader()
    {
        return Reader;
    }

    public static void CreateBaseTablesIfNotExists(this SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            $"CREATE TABLE IF NOT EXISTS entities (id INTEGER PRIMARY KEY, entityId INTEGER, contextId INTEGER, CONSTRAINT entity UNIQUE (entityId, contextId));" +
            $"CREATE TABLE IF NOT EXISTS componentTypes (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, type INTEGER UNIQUE, tableName TEXT);" +
            $"CREATE TABLE IF NOT EXISTS properties (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE, value TEXT);" +
            $"CREATE TABLE IF NOT EXISTS collectors (id INTEGER PRIMARY KEY AUTOINCREMENT, contextId INTEGER, name TEXT, matcherId INTEGER, CONSTRAINT collector UNIQUE (contextId, name));";
        command.ExecuteNonQuery();

        // todo tmp
        var type = typeof(ISqliteComponentTableCreator);
        var types = AppDomain.CurrentDomain.GetAssemblies()
                             .SelectMany(s => s.GetTypes())
                             .Where(p => type.IsAssignableFrom(p) && p.IsClass)
                             .ToArray();

        foreach (var t in types)
        {
            var creator = (ISqliteComponentTableCreator)Activator.CreateInstance(t)!;
            creator.CreateTables(connection);
        }
    }
}