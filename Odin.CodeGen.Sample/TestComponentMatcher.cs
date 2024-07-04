using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public class TestComponentMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(
            c => c.All(w => w.Has<DestroyedComponent>().Has<DestroyedComponent>().Added<DestroyedComponent>()),
            c => c.NotHas<DestroyedComponent>()
        ).Has<DestroyedComponent>().Added<DestroyedComponent>();
    }
}

public class Test2ComponentMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<DestroyedComponent>();
    }
}