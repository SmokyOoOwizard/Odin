using Odin.Abstractions.Contexts;
using Odin.Systems.Features;

namespace Odin.Systems;

public sealed class SystemInstaller<T> : ISystemInstaller where T : ISystem
{
    public ISystem System { get; init; }
    public AEntityContext Context { get; init; }
    public uint Priority { get; init; }

    public SystemInstaller(AEntityContext context, uint priority, T system)
    {
        System = system;
        Context = context;
        Priority = priority;
    }

    public SystemInstaller(AEntityContext context, Feature feature, uint priority, T system)
    {
        System = system;
        Context = context;
        Priority = feature.Priority + priority;
    }
}