using Microsoft.Data.Sqlite;
using Odin.Db.Sqlite.Contexts;
using Odin.Tests.Abstractions.Components;

namespace Odin.Db.Sqlite.Tests.Components;

public class SqliteContextComponentChanges : AComponentChanges
{
    public SqliteContextComponentChanges() : base(
        new SqliteEntityContext(new SqliteConnection("Data Source=:memory:"), nameof(SqliteContextComponentChanges))
    )
    {
    }
}