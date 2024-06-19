using Odin.Abstractions.Components;

namespace OdinSdk.Components;

public record struct ComponentWrapper(int TypeId, IComponent? Component);