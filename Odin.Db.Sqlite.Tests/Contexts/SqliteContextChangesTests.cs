using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Contexts;

namespace Odin.Db.Sqlite.Tests.Contexts;

public class SqliteContextChangesTests : AContextChangesTests
{
    public SqliteContextChangesTests() : base(
        new SqliteEntityContext(new SqliteConnection("Data Source=:memory:"), nameof(SqliteContextChangesTests))
    )
    {
        
    }
}