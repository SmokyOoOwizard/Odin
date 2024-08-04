using Microsoft.Data.Sqlite;
using Odin.Core.Collectors;
using Odin.Core.Entities.Collections;
using Odin.Db.Sqlite.Utils;

namespace Odin.Db.Sqlite.Entities;

internal class SqliteCollector : IEntityCollector
{
    private readonly SqliteConnection _connection;
    private readonly ulong _contextId;

    public string Name { get; }
    public ulong MatcherId { get; }

    public SqliteCollector(SqliteConnection connection, ulong contextId, string name, ulong matcherId)
    {
        Name = name;
        MatcherId = matcherId;
        _connection = connection;
        _contextId = contextId;
    }
    
    public IEntitiesCollection GetEntities()
    {
        var entities = SqlCollectorsUtils.GetEntitiesFromCollector(_connection, _contextId, Name);
        throw new NotImplementedException();
    }
}