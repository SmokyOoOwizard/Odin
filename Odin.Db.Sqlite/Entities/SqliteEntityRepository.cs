using System.Data;
using Microsoft.Data.Sqlite;
using Odin.Abstractions.Collectors;
using Odin.Abstractions.Components;
using Odin.Abstractions.Entities;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Matchers;
using Odin.Db.Sqlite.Utils;
using Odin.Utils;

namespace Odin.Db.Sqlite.Entities;

public class SqliteEntityRepository : IEntityRepository
{
    private readonly ulong _destroyedId = TypeComponentUtils.GetComponentTypeId<DestroyedComponent>();

    // key - name, value - matcher id
    private readonly Dictionary<string, ulong> _collectors = new();

    // key - name, value - collector
    private readonly Dictionary<string, SqliteCollector> _collectorsCache = new();

    private readonly SqliteConnection _connection;
    private readonly ulong _contextId;

    public SqliteEntityRepository(SqliteConnection connection, ulong contextId)
    {
        _connection = connection;
        _contextId = contextId;
        if (_connection.State == ConnectionState.Closed)
        {
            _connection.Open();
            connection.CreateBaseTablesIfNotExists();
        }
    }

    public bool Get<T>(ulong entityId, out T? component) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var serializer = SqlCommands.GetReader();
        var componentWrapper = serializer.Read(_connection, entityId, _contextId, typeId);

        if (componentWrapper.Component == null)
        {
            component = default;
            return false;
        }

        component = (T)componentWrapper.Component;
        return true;
    }

    public bool GetOld<T>(ulong entityId, out T? component) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var serializer = SqlCommands.GetReader();
        var componentWrapper = serializer.Read(_connection, entityId, _contextId, typeId, true);

        if (componentWrapper.Component == null)
        {
            component = default;
            return false;
        }

        component = (T)componentWrapper.Component;
        return true;
    }

    public IEnumerable<ulong> GetEntities()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = $"SELECT entityId FROM entities WHERE contextId = {_contextId.MapToLong()};";
        using var reader = command.ExecuteReader();

        while (reader.Read() && reader.GetValue(0) != DBNull.Value)
        {
            yield return reader.GetInt64(0).MapToUlong();
        }
    }

    public IEnumerable<(ulong, ComponentWrapper[])> GetEntitiesWithComponents()
    {
        throw new NotImplementedException();
    }

    public void Replace<T>(ulong entityId, T? component) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var reader = SqlCommands.GetReader();
        var writer = SqlCommands.GetWriter();
        var deleter = SqlCommands.GetDeleter();

        var old = reader.Read(_connection, entityId, _contextId, typeId);

        if (component == null)
            deleter.Delete(_connection, entityId, _contextId, typeId);
        else
            writer.Write(_connection, entityId, _contextId, new ComponentWrapper(typeId, component));

        if (old.Component == null)
            deleter.Delete(_connection, entityId, _contextId, typeId, true);
        else
            writer.Write(_connection, entityId, _contextId, old, true);
    }

    public void Remove<T>(ulong entityId) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var reader = SqlCommands.GetReader();
        var writer = SqlCommands.GetWriter();
        var deleter = SqlCommands.GetDeleter();

        var old = reader.Read(_connection, entityId, _contextId, typeId);

        deleter.Delete(_connection, entityId, _contextId, typeId);

        if (old.Component == null)
            deleter.Delete(_connection, entityId, _contextId, typeId, true);
        else
            writer.Write(_connection, entityId, _contextId, old, true);
    }

    public IEntityCollector CreateOrGetCollector<T>(string name) where T : AReactiveComponentMatcher
    {
        var matcherId = MatchersRepository.GetMatcherId<T>();

        if (_collectors.ContainsKey(name))
            throw new InvalidOperationException("Collector already exists.");

        _collectors[name] = matcherId;

        SqlCollectorsUtils.CreateCollector(_connection, _contextId, name, matcherId);

        var collector = _collectorsCache[name] = new SqliteCollector(_connection, _contextId, name, matcherId);

        return collector;
    }

    public void DeleteCollector(string name)
    {
        if (!_collectors.ContainsKey(name))
            return;

        _collectors.Remove(name);
        _collectorsCache.Remove(name);

        SqlCollectorsUtils.DeleteCollector(_connection, _contextId, name);
    }

    public ulong CreateEntity()
    {
        lock (_connection)
        {
            ulong lastId = 0;
            {
                using var command = _connection.CreateCommand();

                command.CommandText = "SELECT value FROM properties WHERE name = 'lastEntityId';";
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var p = reader.GetString(0);
                    lastId = long.Parse(p).MapToUlong();
                }
            }

            lastId++;

            {
                using var command = _connection.CreateCommand();

                command.CommandText =
                    $"INSERT OR IGNORE INTO properties (name, value) VALUES ('lastEntityId', {lastId.MapToLong()});" +
                    $"UPDATE properties SET value = '{lastId.MapToLong()}' WHERE name = 'lastEntityId';" +
                    $"INSERT INTO entities (entityId, contextId) VALUES ({lastId.MapToLong()}, {_contextId.MapToLong()});";
                command.ExecuteNonQuery();
            }

            return lastId;
        }
    }

    public void DestroyEntity(ulong entityId)
    {
        using var command = _connection.CreateCommand();

        command.CommandText =
            $"DELETE FROM entities WHERE entityId = {entityId.MapToLong()} AND contextId = {_contextId.MapToLong()};";
        command.ExecuteNonQuery();

        // todo delete components;
    }

    public void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities)
    {
        var reader = SqlCommands.GetReader();
        var writer = SqlCommands.GetWriter();
        var deleter = SqlCommands.GetDeleter();

        var matchers = _collectors.GroupBy(c => c.Value).Select(c => new
        {
            matcherId = c.Key,
            collectors = c.Select(q => q.Key),
            filter = MatchersRepository.GetFilter(c.Key)
        }).ToArray();

        foreach (var (id, changes) in entities)
        {
            if (changes.Any(c => c.TypeId == _destroyedId))
            {
                DestroyEntity(id);
                continue;
            }

            foreach (var component in changes)
            {
                var old = reader.Read(_connection, id, _contextId, component.TypeId);

                if (component.Component == null)
                    deleter.Delete(_connection, id, _contextId, component.TypeId);
                else
                    writer.Write(_connection, id, _contextId, component);

                if (old.Component == null)
                    deleter.Delete(_connection, id, _contextId, component.TypeId, true);
                else
                    writer.Write(_connection, id, _contextId, old, true);
            }

            foreach (var matcher in matchers)
            {
                if (matcher.filter(id, HasComponent, changes))
                {
                    foreach (var collector in matcher.collectors)
                    {
                        SqlCollectorsUtils.AddEntityToCollector(_connection, _contextId, collector, id);
                    }
                }
            }
        }
    }

    private bool HasComponent(ulong entityId, ulong componentId)
    {
        var serializer = SqlCommands.GetReader();
        var componentWrapper = serializer.Read(_connection, entityId, _contextId, componentId);

        return componentWrapper.Component != null;
    }

    public void Clear()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = $"DELETE FROM entities WHERE contextId = {_contextId.MapToLong()};";
        command.ExecuteNonQuery();

        // todo delete components;
    }
}