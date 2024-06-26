using Odin.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace Odin.Systems;

public interface ISystemInstaller
{
    ISystem System { get; }

    AEntityContext Context { get; }

    uint Priority { get; }
}