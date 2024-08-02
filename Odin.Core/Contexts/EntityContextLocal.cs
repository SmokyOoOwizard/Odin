using Odin.Abstractions.Entities;

namespace Odin.Contexts;

public struct EntityContextLocal
{
    public IEntityComponentsRepository Changes { get; set; }
}