using Microsoft.Data.Sqlite;
using Odin.Core.Utils;
using Odin.Db.Sqlite.Entities;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Db.Sqlite.Tests.Entities;

public class SqliteEntityRepositoryTests : AEntityRepositoryTests
{
    public SqliteEntityRepositoryTests() : base(
        new SqliteEntityRepository(
            new SqliteConnection("Data Source=:memory:"),
            EntityContextUtils.ComputeId(nameof(SqliteEntityRepositoryTests)))
    )
    {
    }
}