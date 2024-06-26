namespace OdinSdk.Entities.Matcher;

public static class EntityMatcherLogicExtensions
{
    public static EntityMatcher All(this EntityMatcher matcher, params EntityMatcher[] matchers)
    {
        return matcher;
    }
    
    public static EntityMatcher Not(this EntityMatcher matcher, params EntityMatcher[] matchers)
    {
        return matcher;
    }
    
    public static EntityMatcher Any(this EntityMatcher matcher, params EntityMatcher[] matchers)
    {
        return matcher;
    }
}