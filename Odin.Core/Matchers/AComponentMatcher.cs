using Odin.Core.Abstractions.Matchers;
using Odin.Core.Abstractions.Matchers.Impl;

namespace Odin.Core.Matchers;

public abstract class AComponentMatcher : AComponentMatcherBase
{
    protected ComponentMatcherBuilder Filter()
    {
        return new ComponentMatcherBuilder();
    }
}