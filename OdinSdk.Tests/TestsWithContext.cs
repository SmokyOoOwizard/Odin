using Odin.Abstractions.Contexts;
using OdinSdk.Contexts;

namespace OdinSdk.Tests;

public abstract class TestsWithContext : IDisposable
{
    protected readonly AEntityContext Context;

    public TestsWithContext()
    {
        EntityContexts.Clear();
        Context = new InMemoryEntityContext(GetType().FullName!);
    }
    
    public void Dispose()
    {
        Context.Dispose();
    }
}