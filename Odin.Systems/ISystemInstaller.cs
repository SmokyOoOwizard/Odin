using Odin.Core.Contexts;

namespace Odin.Systems;

public interface ISystemInstaller
{
    ISystem System { get; }

    AEntityContext Context { get; }

    uint Priority { get; }
}