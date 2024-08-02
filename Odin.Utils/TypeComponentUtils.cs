using System.Security.Cryptography;
using System.Text;
using Odin.Core.Abstractions.Components;

namespace Odin.Utils;

public static class TypeComponentUtils
{
    public static ulong GetComponentTypeId<T>() where T : IComponent
    {
        var name = typeof(T).FullName?.Replace("+", ".");
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Type must have a name", nameof(T));

        var byteString = Encoding.Default.GetBytes(name);

        using var hasher = new SHA256Managed();

        var rawHash = hasher.ComputeHash(byteString);

        var hashCodeStart = BitConverter.ToUInt64(rawHash, 0);
        var hashCodeMedium = BitConverter.ToUInt64(rawHash, 8);
        var hashCodeEnd = BitConverter.ToUInt64(rawHash, 24);
        var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

        return hashCode;
    }

    public static string? GetFullName(this Type type)
    {
        var fullName = type.FullName;
        var name = fullName?.Replace("+", ".");

        return name;
    }

    public static ulong GetComponentTypeId(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentNullException(nameof(name));

        var byteString = Encoding.Default.GetBytes(name);

        using var hasher = new SHA256Managed();

        var rawHash = hasher.ComputeHash(byteString);

        var hashCodeStart = BitConverter.ToUInt64(rawHash, 0);
        var hashCodeMedium = BitConverter.ToUInt64(rawHash, 8);
        var hashCodeEnd = BitConverter.ToUInt64(rawHash, 24);
        var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

        return hashCode;
    }
}