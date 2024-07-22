namespace Odin.Abstractions.Collectors.Matcher;

public interface IComponentMatcherRepository
{
    bool HasMatcher(ulong id);
    bool HasMatcher<T>() where T : AComponentMatcherBase;
    string GetMatcherJson(ulong matcherId);
    
    ulong GetMatcherId<T>() where T : AComponentMatcherBase;
    FilterComponentDelegate GetFilter(ulong matcherId);
}