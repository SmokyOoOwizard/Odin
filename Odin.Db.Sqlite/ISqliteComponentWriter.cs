using Microsoft.Data.Sqlite;
using Odin.Core.Components;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentWriter
{
    void Write(SqliteConnection connection, ulong entityId, ulong contextId, ComponentWrapper component, bool old = false);
}