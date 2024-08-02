using Odin.Core.Abstractions.Matchers.Impl;

namespace Odin.Core.Matchers.Extensions;

public static class ComponentMatcherLogicExtensions
{
    public static ReactiveComponentMatcherBuilder All(
        this ReactiveComponentMatcherBuilder matcher,
        params Func<ReactiveComponentMatcherBuilder, ReactiveComponentMatcherBuilder>[] matchers
    )
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder All(
        this ComponentMatcherBuilder matcher,
        params Func<ComponentMatcherBuilder, ComponentMatcherBuilder>[] matchers
    )
    {
        return matcher;
    }

    public static ReactiveComponentMatcherBuilder Not(
        this ReactiveComponentMatcherBuilder matcher,
        params Func<ReactiveComponentMatcherBuilder, ReactiveComponentMatcherBuilder>[] matchers
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

    public static ReactiveComponentMatcherBuilder Any(
        this ReactiveComponentMatcherBuilder matcher,
        params Func<ReactiveComponentMatcherBuilder, ReactiveComponentMatcherBuilder>[] matchers
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