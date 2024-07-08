using Odin.Abstractions.Collectors.Matcher;
using Odin.Abstractions.Components;

namespace Odin.Component.CodeGen.Sample;

public class ComplexComponentMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(
            c => c.All(w => w.Has<DestroyedComponent>().Has<DestroyedComponent>().Added<DestroyedComponent>()),
            c => c.NotHas<DestroyedComponent>()
        ).Has<DestroyedComponent>().Added<DestroyedComponent>();
    }
}

public class HasMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<DestroyedComponent>();
    }
}

public class NotHasMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().NotHas<TestComponent>();
    }
}

public class AllHasMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Has<TestComponent>().Has<DestroyedComponent>();
    }
}

public class AnyHasMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Any(c => c.Has<TestComponent>(),
                     c => c.Has<DestroyedComponent>());
    }
}

public class NotMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Not(c => c.Has<TestComponent>(),
                     c => c.Has<DestroyedComponent>());
    }
}

public class AddedMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Added<TestComponent>();
    }
}

public class RemovedMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().Removed<TestComponent>();
    }
}

public class AnyChangesMatcher : AComponentMatcher
{
    public override void Configure()
    {
        Filter().AnyChanges<TestComponent>();
    }
}

public class EmptyComponentMatcher : AComponentMatcher
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