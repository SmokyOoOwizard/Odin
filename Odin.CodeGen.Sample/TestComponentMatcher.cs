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
        Filter();
    }
}

public class WrapperComponentMatcher
{
    public class Test3ComponentMatcher : AComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<DestroyedComponent>();
        }
    }
}

public class Test4ComponentMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<DestroyedComponent>();
    }
}