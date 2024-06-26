namespace OdinSdk.Entities.Matcher;

public static class EntityMatcherComponentsExtensions
{
    public static EntityMatcher Has<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
    
    public static EntityMatcher NotHas<T>(this EntityMatcher matcher)
    {
        return matcher;
    }
}