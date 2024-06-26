using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentTableCreator
{
    void CreateTables(SqliteConnection connection);
}