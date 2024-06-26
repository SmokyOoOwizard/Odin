using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite.Utils;

internal static class SqlCommands
{
    private static ISqliteComponentDeleter? _deleter;
    private static ISqliteComponentWriter? _writer;
    private static ISqliteComponentReader? _reader;

    public static ISqliteComponentDeleter GetDeleter()
    {
        if (_deleter != null)
            return _deleter;

        var type = typeof(ISqliteComponentDeleter);
        var existsType = AppDomain.CurrentDomain
                                  .GetAssemblies()
                                  .SelectMany(s => s.GetTypes())
                                  .First(p => type.IsAssignableFrom(p) && p.IsClass);

        var deleter = (ISqliteComponentDeleter)Activator.CreateInstance(existsType)!;
        _deleter = deleter;
        return deleter;
    }

    public static ISqliteComponentWriter GetWriter()
    {
        if (_writer != null)
            return _writer;

        var type = typeof(ISqliteComponentWriter);
        var existsType = AppDomain.CurrentDomain
                                  .GetAssemblies()
                                  .SelectMany(s => s.GetTypes())
                                  .First(p => type.IsAssignableFrom(p) && p.IsClass);

        var writer = (ISqliteComponentWriter)Activator.CreateInstance(existsType)!;
        _writer = writer;
        return writer;
    }

    public static ISqliteComponentReader GetReader()
    {
        if (_reader != null)
            return _reader;

        var type = typeof(ISqliteComponentReader);
        var existsType = AppDomain.CurrentDomain
                                  .GetAssemblies()
                                  .SelectMany(s => s.GetTypes())
                                  .First(p => type.IsAssignableFrom(p) && p.IsClass);

        var reader = (ISqliteComponentReader)Activator.CreateInstance(existsType)!;
        _reader = reader;
        return reader;
    }

    public static void CreateBaseTablesIfNotExists(this SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS entities (id INTEGER PRIMARY KEY, entityId INTEGER, contextId INTEGER, CONSTRAINT entity UNIQUE (entityId, contextId));" +
                              $"CREATE TABLE IF NOT EXISTS componentTypes (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, type INTEGER UNIQUE, tableName TEXT);" +
                              $"CREATE TABLE IF NOT EXISTS properties (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE, value TEXT);";
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