using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Contexts;

namespace Odin.Db.Sqlite.Tests.Contexts;

public abstract class ASqliteMultiContextChangesTests : AMultiContextChangesTests
{
    public ASqliteMultiContextChangesTests(SqliteConnection connection) :
        base(
            new SqliteEntityContext(connection, nameof(ASqliteMultiContextChangesTests)),
            new SqliteEntityContext(connection, nameof(ASqliteMultiContextChangesTests) + "2")
        )
    {
    }
}