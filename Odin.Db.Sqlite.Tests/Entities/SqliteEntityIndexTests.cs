using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Db.Sqlite.Tests.Entities;

public class SqliteEntityIndexTests : AEntityIndexTests
{
    public SqliteEntityIndexTests() : base(
        new SqliteEntityContext(new SqliteConnection("Data Source=:memory:"), nameof(SqliteEntityIndexTests))
    )
    {
    }
}