using Odin.Core.Contexts.Impl;
using Odin.Tests.Abstractions.Entities;

namespace Odin.Core.Tests.Entities;

public class InMemoryEntityIndexTests : AEntityIndexTests
{
    public InMemoryEntityIndexTests() : base(
        new InMemoryEntityContext(nameof(InMemoryEntityIndexTests))
    )
    {
    }
}