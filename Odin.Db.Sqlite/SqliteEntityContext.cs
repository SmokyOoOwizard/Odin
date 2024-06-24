using Microsoft.Data.Sqlite;
using Odin.Abstractions.Contexts;
using Odin.Abstractions.Contexts.Utils;

namespace Odin.Db.Sqlite;

public class SqliteEntityContext : AEntityContext
{
    public override string Name { get; }
    
    public override ulong Id { get; }

    public SqliteEntityContext(SqliteConnection connection, string name)
    {
        Id = EntityContextUtils.ComputeId(name);
        Name = name;
    }
}