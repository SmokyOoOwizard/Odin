using Odin.Core.Repositories.Entities;

namespace Odin.Core.Contexts;

public struct EntityContextLocal
{
    public IEntityComponentsRepository Changes { get; set; }
}