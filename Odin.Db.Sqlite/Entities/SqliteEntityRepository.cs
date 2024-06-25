using Microsoft.Data.Sqlite;
using Odin.Abstractions.Components;
using Odin.Abstractions.Components.Utils;
using Odin.Abstractions.Entities;
using Odin.Db.Sqlite.Utils;

namespace Odin.Db.Sqlite.Entities;

public class SqliteEntityRepository : IEntityRepository
{
    private readonly ulong _destroyedId = TypeComponentUtils.GetComponentTypeId<DestroyedComponent>();

    private readonly SqliteConnection _connection;

    public SqliteEntityRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public bool Get<T>(ulong entityId, out T? component) where T : IComponent
    {
        throw new NotImplementedException();
    }

    public bool GetOld<T>(ulong entityId, out T? component) where T : IComponent
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ulong> GetEntities()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = "SELECT id FROM entities;";
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
        throw new NotImplementedException();
    }

    public void Remove<T>(ulong entityId) where T : IComponent
    {
        throw new NotImplementedException();
    }

    public ulong CreateEntity()
    {
        lock (_connection)
        {
            ulong lastId = 0;
            {
                using var command = _connection.CreateCommand();

                command.CommandText = "SELECT id FROM properties WHERE name = 'lastEntityId';";
                using var reader = command.ExecuteReader();

                if (reader.Read())
                    lastId = reader.GetInt64(0).MapToUlong();
            }

            lastId++;

            {
                using var command = _connection.CreateCommand();

                command.CommandText =
                    $"INSERT OR IGNORE INTO properties (name, value) VALUES ('lastEntityId', {lastId.MapToLong()});" +
                    $"UPDATE properties SET value = {lastId.MapToLong()} WHERE name = 'lastEntityId';" +
                    $"INSERT INTO entities (id) VALUES ({lastId.MapToLong()});";
                command.ExecuteNonQuery();
            }

            return lastId;
        }
    }

    public void DestroyEntity(ulong entityId)
    {
        using var command = _connection.CreateCommand();

        command.CommandText = $"DELETE FROM entities WHERE id = {entityId.MapToLong()};";
        command.ExecuteNonQuery();

        // todo delete components;
    }

    public void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities)
    {
        foreach (var (id, components) in entities)
        {
            if (components.Any(c => c.TypeId == _destroyedId))
            {
                DestroyEntity(id);
                continue;
            }

            // todo apply components changes
        }
    }

    public void Apply((ulong, ComponentWrapper[]) entity)
    {
        if (entity.Item2.Any(c => c.TypeId == _destroyedId))
        {
            DestroyEntity(entity.Item1);
            return;
        }

        // todo apply components changes
    }

    public void Clear()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = "DELETE FROM entities;";
        command.ExecuteNonQuery();

        // todo delete components;
    }
}