using Microsoft.Data.Sqlite;

namespace Odin.Db.Sqlite.Tests.Contexts;

public class SqliteMultiContextChangesTests : ASqliteMultiContextChangesTests
{
    public SqliteMultiContextChangesTests() : base(new SqliteConnection("Data Source=:memory:"))
    {
    }
}