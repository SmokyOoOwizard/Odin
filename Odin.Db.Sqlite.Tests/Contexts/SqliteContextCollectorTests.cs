using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Db.Sqlite.Tests.Contexts;

public class SqliteContextCollectorTests : AEntityRepositoryCollectorTests
{
    public SqliteContextCollectorTests() : base(
        new SqliteEntityContext(new SqliteConnection("Data Source=:memory:"), nameof(SqliteContextCollectorTests))
    )
    {
    }
}