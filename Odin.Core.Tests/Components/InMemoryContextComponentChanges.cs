using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Components;

namespace Odin.Core.Tests.Components;

public class InMemoryContextComponentChanges : AComponentChanges
{
    public InMemoryContextComponentChanges() : base(new InMemoryEntityContext(nameof(InMemoryContextComponentChanges)))
    {
    }
}