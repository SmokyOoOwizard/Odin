namespace Odin.Abstractions.Collectors.Matcher;

public static class ComponentMatcherLogicExtensions
{
    public static ComponentMatcherBuilder All(
        this ComponentMatcherBuilder matcher,
        params Func<ComponentMatcherBuilder, ComponentMatcherBuilder>[] matchers
    )
    {
        return matcher;
    }

    public static ComponentMatcherBuilder Not(
        this ComponentMatcherBuilder matcher,
        params Func<ComponentMatcherBuilder, ComponentMatcherBuilder>[] matchers
    )
    {
        return matcher;
    }

    public static ComponentMatcherBuilder Any(
        this ComponentMatcherBuilder matcher,
        params Func<ComponentMatcherBuilder, ComponentMatcherBuilder>[] matchers
    )
    {
        return matcher;
    }
}