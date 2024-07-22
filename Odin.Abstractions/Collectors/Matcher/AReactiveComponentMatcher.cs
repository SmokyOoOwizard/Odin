namespace Odin.Abstractions.Collectors.Matcher;

public abstract class AReactiveComponentMatcher : AComponentMatcherBase
{
    protected ReactiveComponentMatcherBuilder Filter()
    {
        return new ReactiveComponentMatcherBuilder();
    }
}