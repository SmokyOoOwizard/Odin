namespace Odin.Abstractions.Collectors.Matcher;

public static class ComponentMatcherReactiveExtensions
{
    public static ComponentMatcherBuilder Added<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder Removed<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder Replaced<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
    
    public static ComponentMatcherBuilder AnyChanges<T>(this ComponentMatcherBuilder matcher)
    {
        return matcher;
    }
}