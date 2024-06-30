using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public class TestComponentMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(
            c => c.NotHas<DestroyedComponent>(),
            c => c.All(w => w.Has<DestroyedComponent>())
        );
    }
}