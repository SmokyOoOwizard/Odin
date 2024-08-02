using Odin.Contexts;
using Odin.Tests.Abstractions.Components;

namespace Odin.Tests.Components;

public class InMemoryContextComponentChanges : AComponentChanges
{
    public InMemoryContextComponentChanges() : base(new InMemoryEntityContext(nameof(InMemoryContextComponentChanges)))
    {
    }
}