namespace Odin.Abstractions.Collectors.Matcher.Extensions;

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