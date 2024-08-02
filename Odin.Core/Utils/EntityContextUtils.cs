using System.Security.Cryptography;
using System.Text;

namespace Odin.Core.Utils;

public static class EntityContextUtils
{
    public static ulong ComputeId(string contextName)
    {
        if(string.IsNullOrWhiteSpace(contextName))
            throw new ArgumentException("Context name cannot be empty", nameof(contextName)); 

        var byteString = Encoding.Default.GetBytes(contextName);

        using var hasher = new SHA256Managed();

        var rawHash = hasher.ComputeHash(byteString);
        
        var hashCodeStart = BitConverter.ToUInt64(rawHash, 0);
        var hashCodeMedium = BitConverter.ToUInt64(rawHash, 8);
        var hashCodeEnd = BitConverter.ToUInt64(rawHash, 24);
        var hashCode = hashCodeStart ^ hashCodeMedium ^ hashCodeEnd;

        return hashCode;
    }
}