using System.Collections;
using Microsoft.Data.Sqlite;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Matchers;
using Odin.Core.Repositories.Entities;
using Odin.Utils;

namespace Odin.Db.Sqlite.Entities.Collections;

public class SqliteEntitiesCollection : IEntitiesCollection
{
    private readonly SqliteConnection _connection;
    private readonly string _query;
    private readonly ulong _contextId;
    private readonly SqliteEntityRepository _storage;
    private readonly IEntityComponentsRepository _changes;
    private readonly int _batchSize;

    private readonly Queue<Entity> _queue = new();

    public SqliteEntitiesCollection(
        SqliteConnection connection,
        string query,
        ulong contextId,
        SqliteEntityRepository storage,
        IEntityComponentsRepository changes,
        int batchSize = 100
    )
    {
        _connection = connection;
        _query = query;
        _contextId = contextId;
        _storage = storage;
        _changes = changes;
        _batchSize = batchSize;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = _query;
        using var reader = command.ExecuteReader();

        bool hasNext;
        do
        {
            hasNext = FillQueue(reader);

            while (_queue.Count > 0)
            {
                yield return _queue.Dequeue();
            }
        } while (hasNext);
    }

    public IRawEntitiesCollection Filter<T>() where T : AComponentMatcher
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private bool FillQueue(SqliteDataReader reader)
    {
        var breakByLimit = false;
        while (reader.Read())
        {
            var id = reader.GetInt64(0).MapToUlong();

            var entity = new Entity(new(id, _contextId), _storage, _changes);

            _queue.Enqueue(entity);

            if (_queue.Count >= _batchSize)
            {
                breakByLimit = true;
                break;
            }
        }

        return breakByLimit;
    }
}