using Microsoft.Data.Sqlite;
using Odin.Abstractions.Entities;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentWriter
{
    void Write(SqliteConnection connection, ulong entityId, ulong contextId, ComponentWrapper component, bool old = false);
}