using Microsoft.Data.Sqlite;
using Odin.Abstractions.Components;
using Odin.Abstractions.Entities;
using Odin.Db.Sqlite.Utils;

namespace Odin.Db.Sqlite.Entities;

public class SqliteEntityRepository : IEntityRepository
{
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

        while (reader.Read())
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

                command.CommandText = $"UPDATE properties SET value = {lastId.MapToLong()} WHERE name = 'lastEntityId';";
                command.ExecuteNonQuery();
            }

            {
                using var command = _connection.CreateCommand();

                command.CommandText = $"INSERT INTO entities (id) VALUES ({lastId.MapToLong()});";
                command.ExecuteNonQuery();
            }

            return lastId;
        }
    }

    public void DestroyEntity(ulong entityId)
    {
        using var command = _connection.CreateCommand();

        command.CommandText = $"DELETE FROM entities WHERE id = {entityId};";
        command.ExecuteNonQuery();

        // todo delete components;
    }

    public void Apply(IEnumerable<(ulong, ComponentWrapper[])> entities)
    {
        throw new NotImplementedException();
    }

    public void Apply((ulong, ComponentWrapper[]) entity)
    {
        throw new NotImplementedException();
    }

    public void Clear()
    {
        using var command = _connection.CreateCommand();

        command.CommandText = "DELETE FROM entities;";
        command.ExecuteNonQuery();

        // todo delete components;
    }
}