using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Data.Sqlite;
using Odin.Core.Abstractions.Components;
using Odin.Core.Abstractions.Components.Declarations;
using Odin.Core.Components;
using Odin.Core.Components.Declaration;
using Odin.Utils;

namespace Odin.Db.Sqlite.Utils;

public class SqliteComponentReader : ISqliteComponentReader
{
    public ComponentWrapper Read(
        SqliteConnection connection,
        ulong entityId,
        ulong contextId,
        ulong componentTypeId,
        bool old = false
    )
    {
        if (!ComponentDeclarations.TryGet(componentTypeId, out var componentDeclaration))
            throw new Exception("Unknown component type");

        var tableName = (old ? "__old_" : "") + componentDeclaration.Name.Replace('.', '_');

        var sb = new StringBuilder();
        sb.Append($"SELECT {string.Join(", ", componentDeclaration.Fields.Select(x => x.Name))} ");

        sb.Append($"FROM {tableName} WHERE entityId = {entityId} AND contextId = {contextId.MapToLong()};");

        using var command = connection.CreateCommand();

        command.CommandText = sb.ToString();
        using var reader = command.ExecuteReader();

        if (!reader.Read())
            return new ComponentWrapper(componentTypeId, null);

        var component = (IComponent)Activator.CreateInstance(componentDeclaration.Type)!;

        var componentRef = component;
        var componentPtr = GCHandle.Alloc(componentRef, GCHandleType.Pinned);
        var componentAddr = componentPtr.AddrOfPinnedObject();

        try
        {
            var column = 0;
            foreach (var fieldDeclaration in componentDeclaration.Fields)
            {
                if (fieldDeclaration.CollectionType == ECollectionType.None)
                    ReadValue(componentAddr, fieldDeclaration, reader, column);
                else
                    ReadCollectionValue(componentAddr, fieldDeclaration, reader, column);

                column++;
            }
        }
        finally
        {
            componentPtr.Free();
        }


        return new ComponentWrapper(componentTypeId, component);
    }

    private unsafe void ReadValue(
        IntPtr componentPtr,
        ComponentFieldDeclaration fieldDeclaration,
        SqliteDataReader reader,
        int column
    )
    {
        var offset = fieldDeclaration.Offset;

        var ptr = componentPtr + offset;

        switch (fieldDeclaration.Type)
        {
            case EFieldType.Int:
            case EFieldType.Int32:
                var intValue = reader.GetInt32(column);

                *(int*)ptr = intValue;
                break;
            case EFieldType.Int8:
                var sbyteValue = reader.GetByte(column).MapToSbyte();

                *(sbyte*)ptr = sbyteValue;
                break;
            case EFieldType.Int16:
                var shortValue = reader.GetInt16(column);

                *(short*)ptr = shortValue;
                break;
            case EFieldType.Int64:
                var longValue = reader.GetInt64(column);

                *(long*)ptr = longValue;
                break;
            case EFieldType.UInt8:
                var byteValue = reader.GetByte(column);

                *(byte*)ptr = byteValue;
                break;
            case EFieldType.UInt16:
                var ushortValue = reader.GetInt16(column).MapToUshort();

                *(ushort*)ptr = ushortValue;
                break;
            case EFieldType.UInt32:
                var uintValue = reader.GetInt32(column).MapToUint();

                *(uint*)ptr = uintValue;
                break;
            case EFieldType.UInt64:
                var ulongValue = reader.GetInt64(column).MapToUlong();

                *(ulong*)ptr = ulongValue;
                break;
            case EFieldType.Float:
                var floatValue = reader.GetFloat(column);

                *(float*)ptr = floatValue;
                break;
            case EFieldType.Double:
                var doubleValue = reader.GetDouble(column);

                *(double*)ptr = doubleValue;
                break;
            case EFieldType.Bool:
                var boolValue = reader.GetBoolean(column);

                *(bool*)ptr = boolValue;
                break;
            case EFieldType.String:
            case EFieldType.Complex:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ReadCollectionValue(
        IntPtr componentPtr,
        ComponentFieldDeclaration fieldDeclaration,
        SqliteDataReader reader,
        int row
    )
    {
    }
}