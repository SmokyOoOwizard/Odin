using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentDeleter
{
    void Delete(SqliteConnection connection, ulong entityId, ulong contextId, ulong componentTypeId, bool old = false);
}