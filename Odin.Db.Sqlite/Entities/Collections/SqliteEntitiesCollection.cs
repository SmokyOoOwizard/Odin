using System.Collections;
using Microsoft.Data.Sqlite;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Matchers;

namespace Odin.Db.Sqlite.Entities.Collections;

public class SqliteEntitiesCollection : IEntitiesCollection
{
    private readonly SqliteConnection _connection;
    private readonly string _query;
    private readonly int _batchSize;

    public SqliteEntitiesCollection(
        SqliteConnection connection,
        string query,
        int batchSize = 100
    )
    {
        _connection = connection;
        _query = query;
        _batchSize = batchSize;
    }

    public IEnumerator<Entity> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public IRawEntitiesCollection Filter<T>() where T : AComponentMatcher
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}