using Microsoft.Data.Sqlite;
using Odin.Abstractions.Entities;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentReader
{
    ComponentWrapper Read(SqliteConnection connection, ulong entityId, ulong contextId, ulong componentTypeId, bool old = false);
}