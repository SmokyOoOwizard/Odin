namespace Odin.Abstractions.Collectors.Matcher;

public abstract class AComponentMatcher : AComponentMatcherBase
{
    protected ComponentMatcherBuilder Filter()
    {
        return new ComponentMatcherBuilder();
    }
}