﻿using System.Security.Cryptography;
using System.Text;

namespace Odin.Abstractions.Components.Utils;

public static class TypeComponentUtils
{
    public static ulong GetComponentTypeId<T>() where T : IComponent
    {
        var name = typeof(T).FullName;
        if(string.IsNullOrWhiteSpace(name))
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
}