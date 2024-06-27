namespace Odin.Abstractions.Collectors.Matcher;

public abstract class AComponentMatcher
{
    public abstract void Configure();

    protected ComponentMatcherBuilder Filter()
    {
        return new ComponentMatcherBuilder();
    }
}