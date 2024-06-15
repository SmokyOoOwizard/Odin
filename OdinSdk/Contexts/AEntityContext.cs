namespace OdinSdk.Contexts;

public abstract class AEntityContext: IDisposable
{
    public abstract string Name { get; }

    public abstract ulong Id { get; }

    internal EntityContextLocal Local => EntityContexts.GetContext(Id);

    public virtual void Dispose()
    {
    }
}