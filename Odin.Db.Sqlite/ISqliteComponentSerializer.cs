using Microsoft.Data.Sqlite;
using Odin.Abstractions.Entities;

namespace Odin.Db.Sqlite;

public interface ISqliteComponentSerializer
{
    void Write(SqliteConnection connection, ulong entityId, ComponentWrapper component, bool old = false);
    void Delete(SqliteConnection connection, ulong entityId, ulong componentTypeId, bool old = false);
    ComponentWrapper Read(SqliteConnection connection, ulong entityId, ulong componentTypeId, bool old = false);
}