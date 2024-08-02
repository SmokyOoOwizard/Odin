using Odin.Core.Abstractions.Matchers;
using Odin.Core.Matchers;

namespace Odin.Core.Repositories.Matchers;

public interface IComponentMatcherRepository
{
    bool HasMatcher(ulong id);
    bool HasMatcher<T>() where T : AComponentMatcherBase;
    string GetMatcherJson(ulong matcherId);
    
    ulong GetMatcherId<T>() where T : AComponentMatcherBase;
    FilterComponentDelegate GetFilter(ulong matcherId);
}