using Odin.Abstractions.Entities;

namespace Odin.Abstractions.Collectors;

public delegate bool FilterComponentDelegate(ulong entityId, HasComponentDelegate hasFunc, ComponentWrapper[] changes);