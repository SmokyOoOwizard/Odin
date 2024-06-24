using Microsoft.Data.Sqlite;
using Odin.Abstractions.Contexts;
using Odin.Abstractions.Contexts.Utils;
using Odin.Db.Sqlite.Entities;
using Odin.Db.Sqlite.Utils;

namespace Odin.Db.Sqlite.Contexts;

public class SqliteEntityContext : AEntityContext
{
    private readonly SqliteConnection _connection;

    public override string Name { get; }

    public sealed override ulong Id { get; }

    public SqliteEntityContext(SqliteConnection connection, string name)
    {
        _connection = connection;
        Id = EntityContextUtils.ComputeId(name);
        Name = name;
        
        connection.Open();
        connection.CreateBaseTablesIfNotExists();
        
        var rep = new SqliteEntityRepository(_connection);
        EntityContextsRepository.AddRepository(Id, rep);
    }

    public override void Dispose()
    {
        EntityContextsRepository.RemoveRepository(Id);
        _connection.Dispose();
    }
}