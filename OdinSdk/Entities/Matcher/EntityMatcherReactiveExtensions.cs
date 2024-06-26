namespace OdinSdk.Entities.Matcher;

public static class EntityMatcherReactiveExtensions
{
    public static EntityMatcher Added<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
    
    public static EntityMatcher Removed<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
    
    public static EntityMatcher Replaced<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
    
    public static EntityMatcher AnyChanges<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
}