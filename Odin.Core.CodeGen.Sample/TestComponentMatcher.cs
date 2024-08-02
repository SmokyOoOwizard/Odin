using Odin.Core.Abstractions.Matchers;
using Odin.Core.Components;
using Odin.Core.Matchers.Extensions;

namespace Odin.Core.CodeGen.Sample;

public class ComplexComponentMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(
            c => c.All(w => w.Has<DestroyedComponent>().Has<DestroyedComponent>().Added<DestroyedComponent>()),
            c => c.NotHas<DestroyedComponent>()
        ).Has<DestroyedComponent>().Added<DestroyedComponent>();
    }
}

public class HasMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<DestroyedComponent>();
    }
}

public class NotHasMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().NotHas<TestComponent>();
    }
}

public class AllHasMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<TestComponent>().Has<DestroyedComponent>();
    }
}

public class AnyHasMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(c => c.Has<TestComponent>(),
                     c => c.Has<DestroyedComponent>());
    }
}

public class NotMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Not(c => c.Has<TestComponent>(),
                     c => c.Has<DestroyedComponent>());
    }
}

public class AddedMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Added<TestComponent>();
    }
}

public class RemovedMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Removed<TestComponent>();
    }
}

public class AnyChangesMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().AnyChanges<TestComponent>();
    }
}

public class EmptyComponentMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter();
    }
}

public class WrapperComponentMatcher
{
    public class Test3ComponentMatcher : AReactiveComponentMatcher
    {
        public override void Configure()
        {
            Filter().Has<DestroyedComponent>();
        }
    }
}

public class Test4ComponentMatcher : AReactiveComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<DestroyedComponent>();
    }
}