using Odin.Abstractions.Entities;
using OdinSdk.Components;

namespace OdinSdk.Contexts;

public struct EntityContextLocal
{
    public IEntityRepository Changes { get; set; }
}