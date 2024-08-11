using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Data.Sqlite;
using Odin.Core.Abstractions.Components.Declarations;
using Odin.Core.Components;
using Odin.Core.Components.Declaration;
using Odin.Utils;

namespace Odin.Db.Sqlite.Utils;

public class SqliteComponentWriter : ISqliteComponentWriter
{
    public void Write(
        SqliteConnection connection,
        ulong entityId,
        ulong contextId,
        ComponentWrapper component,
        bool old = false
    )
    {
        var componentRef = component.Component;
        var componentPtr = GCHandle.Alloc(componentRef, GCHandleType.Pinned);
        var componentAddr = componentPtr.AddrOfPinnedObject();

        try
        {
            if (!ComponentDeclarations.TryGet(component.TypeId, out var componentDeclaration))
                throw new Exception("Unknown component type");

            var tableName = (old ? "__old_" : "") + componentDeclaration.Name.Replace('.', '_');

            var sb = new StringBuilder();
            var valuesSb = new StringBuilder();
            valuesSb.Append($"VALUES ({entityId}, {contextId.MapToLong()}");

            sb.Append($"INSERT OR REPLACE INTO {tableName} (entityId, contextId");

            foreach (var fieldDeclaration in componentDeclaration.Fields)
            {
                var name = fieldDeclaration.Name;

                sb.Append($", {name}");

                string value;

                if (fieldDeclaration.CollectionType == ECollectionType.None)
                    value = GetValue(componentAddr, fieldDeclaration);
                else
                    value = GetCollectionValue(componentAddr, fieldDeclaration);

                valuesSb.Append($", {value}");
            }

            sb.Append(") ");
            valuesSb.Append(");");

            sb.Append(valuesSb);

            using var command = connection.CreateCommand();


            command.CommandText = sb.ToString();
            command.ExecuteNonQuery();
        }
        finally
        {
            componentPtr.Free();
        }
    }

    private unsafe string GetValue(IntPtr componentPtr, ComponentFieldDeclaration fieldDeclaration)
    {
        var offset = fieldDeclaration.Offset;

        var ptr = componentPtr + offset;

        var str = fieldDeclaration.Type switch
        {
            EFieldType.Int => (*(int*)ptr).ToString(),
            EFieldType.Int8 => (*(sbyte*)ptr).ToString(),
            EFieldType.Int16 => (*(short*)ptr).ToString(),
            EFieldType.Int32 => (*(int*)ptr).ToString(),
            EFieldType.Int64 => (*(long*)ptr).ToString(),
            EFieldType.UInt8 => (*(byte*)ptr).ToString(),
            EFieldType.UInt16 => (*(ushort*)ptr).MapToShort().ToString(),
            EFieldType.UInt32 => (*(uint*)ptr).MapToInt().ToString(),
            EFieldType.UInt64 => (*(ulong*)ptr).MapToLong().ToString(),
            EFieldType.Float => (*(float*)ptr).ToString(CultureInfo.InvariantCulture),
            EFieldType.Double => (*(double*)ptr).ToString(CultureInfo.InvariantCulture),
            EFieldType.Bool => (*(bool*)ptr).ToString(),
            _ => throw new ArgumentOutOfRangeException()
        };

        return str;
    }

    private string GetCollectionValue(IntPtr componentPtr, ComponentFieldDeclaration fieldDeclaration)
    {
        return "[]";
    }
}