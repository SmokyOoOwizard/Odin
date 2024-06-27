namespace Odin.Abstractions.Collectors.Matcher;

public static class ComponentMatcherComponentsExtensions
{
    public static ComponentMatcherBuilder Has<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder NotHas<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
}