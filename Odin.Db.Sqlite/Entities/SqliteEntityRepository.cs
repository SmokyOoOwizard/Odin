using System.Data;
using Microsoft.Data.Sqlite;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Matchers;
using Odin.Core.Collectors;
using Odin.Core.Components;
using Odin.Core.Entities;
using Odin.Core.Entities.Collections;
using Odin.Core.Indexes;
using Odin.Core.Repositories.Entities;
using Odin.Core.Repositories.Matchers.Impl;
using Odin.Db.Sqlite.Entities.Collections;
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

    public bool Get<T>(Entity entity, out T? component) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var serializer = SqlCommands.GetReader();
        var componentWrapper = serializer.Read(_connection, entity.Id.Id, _contextId, typeId);

        if (componentWrapper.Component == null)
        {
            component = default;
            return false;
        }

        component = (T)componentWrapper.Component;
        return true;
    }

    public bool GetOld<T>(Entity entity, out T? component) where T : IComponent
    {
        var typeId = TypeComponentUtils.GetComponentTypeId<T>();

        var serializer = SqlCommands.GetReader();
        var componentWrapper = serializer.Read(_connection, entity.Id.Id, _contextId, typeId, true);

        if (componentWrapper.Component == null)
        {
            component = default;
            return false;
        }

        component = (T)componentWrapper.Component;
        return true;
    }

    public IEntitiesCollection GetEntities(IEntityComponentsRepository? changes = default)
    {
        var query = $"SELECT entityId FROM entities WHERE contextId = {_contextId.MapToLong()};";
        return new SqliteEntitiesCollection(_connection, query);
    }

    public void Replace<T>(Entity entity, T? component) where T : IComponent
    {
        var entityId = entity.Id.Id;
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

    public void Remove<T>(Entity entity) where T : IComponent
    {
        var entityId = entity.Id.Id;
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
            return _collectorsCache[name];

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

    public IIndexModule GetIndex(ulong componentId)
    {
        throw new NotImplementedException();
    }

    public void DestroyEntity(Entity entity)
    {
        var entityId = entity.Id.Id;
        using var command = _connection.CreateCommand();

        command.CommandText =
            $"DELETE FROM entities WHERE entityId = {entityId.MapToLong()} AND contextId = {_contextId.MapToLong()};";
        command.ExecuteNonQuery();

        // todo delete components;
    }

    public Entity CreateEntity()
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

            return new Entity(new(lastId, _contextId), this, this);
        }
    }

    public void Apply(IEntitiesCollection entities)
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

        foreach (var entity in entities)
        {
            var id = entity.Id.Id;

            var changes = entity.Changes.GetComponents(entity);

            if (changes.Any(c => c.TypeId == _destroyedId))
            {
                DestroyEntity(entity);
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

            var ownEntity = new Entity(entity.Id, this, entity.Changes);

            foreach (var matcher in matchers)
            {
                if (matcher.filter(ownEntity))
                {
                    foreach (var collector in matcher.collectors)
                    {
                        SqlCollectorsUtils.AddEntityToCollector(_connection, _contextId, collector, id);
                    }
                }
            }
        }
    }

    public void Clear()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = $"DELETE FROM entities WHERE contextId = {_contextId.MapToLong()};";
        command.ExecuteNonQuery();

        // todo delete components;
    }

    public bool Has(Entity entity, ulong componentId)
    {
        var serializer = SqlCommands.GetReader();
        var component = serializer.Read(_connection, entity.Id.Id, _contextId, componentId);

        return component.Component != null;
    }

    public bool WasRemoved(Entity entity, ulong componentId)
    {
        var serializer = SqlCommands.GetReader();
        var component = serializer.Read(_connection, entity.Id.Id, _contextId, componentId);
        var oldComponent = serializer.Read(_connection, entity.Id.Id, _contextId, componentId, true);

        return component.Component == null && oldComponent.Component != null;
    }

    public ComponentWrapper[] GetComponents(Entity entity)
    {
        throw new NotImplementedException();
    }
}