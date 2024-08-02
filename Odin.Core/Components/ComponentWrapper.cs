using Odin.Core.Abstractions.Components;

namespace Odin.Core.Components;

public record struct ComponentWrapper(ulong TypeId, IComponent? Component);