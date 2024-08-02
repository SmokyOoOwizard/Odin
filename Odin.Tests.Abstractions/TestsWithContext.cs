using Odin.Abstractions.Contexts;
using Odin.Contexts;

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
        EntityContexts.Clear();
        Context.Dispose();
    }
}