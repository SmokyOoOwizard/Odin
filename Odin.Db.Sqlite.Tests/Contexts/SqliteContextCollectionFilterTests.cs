using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Db.Sqlite.Tests.Contexts;

public class SqliteContextCollectionFilterTests : AEntityCollectionFilterTests
{
    public SqliteContextCollectionFilterTests() : base(
        new SqliteEntityContext(new SqliteConnection("Data Source=:memory:"), nameof(SqliteContextCollectionFilterTests))
    )
    {
    }
}