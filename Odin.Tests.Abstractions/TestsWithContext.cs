using Odin.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace Odin.Tests.Abstractions;

public abstract class ATestsWithContext : IDisposable
{
    protected readonly AEntityContext Context;

    public ATestsWithContext(AEntityContext context)
    {
        Context = context;
        EntityContexts.Clear();
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}