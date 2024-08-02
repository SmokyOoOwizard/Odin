using Odin.Core.Abstractions.Matchers.Impl;

namespace Odin.Core.Abstractions.Matchers
{
    public abstract class AReactiveComponentMatcher : AComponentMatcherBase
    {
        protected ReactiveComponentMatcherBuilder Filter()
        {
            return new ReactiveComponentMatcherBuilder();
        }
    }
}