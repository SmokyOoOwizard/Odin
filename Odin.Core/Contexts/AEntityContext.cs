namespace Odin.Core.Contexts;

public abstract class AEntityContext : IDisposable
{
    public abstract string Name { get; }

    public abstract ulong Id { get; }

    public virtual void Dispose()
    {
    }
}