using Odin.Tests.Abstractions.Components;
using OdinSdk.Contexts;

namespace OdinSdk.Tests.Components;

public class InMemoryContextComponentChanges : AComponentChanges
{
    public InMemoryContextComponentChanges() : base(new InMemoryEntityContext(nameof(InMemoryContextComponentChanges)))
    {
    }
}