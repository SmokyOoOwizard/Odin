using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite.Utils;

internal static class SqlCommands
{
    public static void CreateBaseTablesIfNotExists(this SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"CREATE TABLE IF NOT EXISTS entities (id INTEGER PRIMARY KEY);" +
                              $"CREATE TABLE IF NOT EXISTS componentTypes (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, type INTEGER UNIQUE, tableName TEXT);" +
                              $"CREATE TABLE IF NOT EXISTS properties (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT UNIQUE, value TEXT);";
        command.ExecuteNonQuery();
    }
}