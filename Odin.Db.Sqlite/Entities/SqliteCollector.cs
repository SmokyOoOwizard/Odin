using Microsoft.Data.Sqlite;
using Odin.Core.Collectors;
using Odin.Core.Entities.Collections;
using Odin.Core.Repositories.Entities;
using Odin.Db.Sqlite.Entities.Collections;
using Odin.Db.Sqlite.Utils;

namespace Odin.Db.Sqlite.Entities;

internal class SqliteCollector : IEntityCollector
{
    private readonly SqliteConnection _connection;
    private readonly ulong _contextId;
    private readonly SqliteEntityRepository _storage;
    private readonly IEntityComponentsRepository _changes;

    public string Name { get; }
    public ulong MatcherId { get; }

    public SqliteCollector(
        SqliteConnection connection,
        ulong contextId,
        string name,
        ulong matcherId,
        SqliteEntityRepository storage,
        IEntityComponentsRepository changes
    )
    {
        Name = name;
        MatcherId = matcherId;
        _connection = connection;
        _contextId = contextId;
        _storage = storage;
        _changes = changes;
    }

    public IEntitiesCollection GetEntities()
    {
        var tableName = SqlCollectorsUtils.GetTableName(_contextId, Name);
        var query = $"SELECT entityId FROM {tableName};";

        var collection = new SqliteEntitiesCollection(_connection, query, _contextId, _storage, _changes);

        return collection;
    }
}