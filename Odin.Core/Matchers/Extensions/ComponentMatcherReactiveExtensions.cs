using Odin.Core.Abstractions.Matchers.Impl;

namespace Odin.Core.Matchers.Extensions;

public static class ComponentMatcherReactiveExtensions
{
    public static ReactiveComponentMatcherBuilder Added<T>(this ReactiveComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ReactiveComponentMatcherBuilder Removed<T>(this ReactiveComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ReactiveComponentMatcherBuilder AnyChanges<T>(this ReactiveComponentMatcherBuilder matcher)
    {
        return matcher;
    }
}